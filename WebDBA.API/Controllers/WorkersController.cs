using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebDBA.API.Interfaces;
using WebDBA.API.Models;
using WebDBA.Migrator.Migration;

namespace WebDBA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidationService _validationService;

        public WorkersController(AppDbContext context, IValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
        {
            try
            {
                var worker = await _context.Workers.ToListAsync();

                return Ok(worker);
            }
            catch (DbException ex)
            {
                return BadRequest(new { Error = $"Произошла ошибка при соединении с базой данных: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("with-latest-history")]
        public async Task<ActionResult<IEnumerable<WorkerWithLatestHistoryDto>>> GetWorkersWithLatestHistory()
        {
            try
            {
                var workers = await GetActiveWorkersAsync();
                return Ok(workers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("with-latest-history/{id}")]
        public async Task<ActionResult<IEnumerable<WorkerWithLatestHistoryDto>>> GetWorkersWithLatestHistoryId(string id)
        {
            try
            {
                var worker = await GetActiveWorkerByIdAsync(id);
                if (worker == null)
                {
                    return NotFound(new { Error = $"Активный сотрудник с ID '{id}' не найден" });
                }
                return Ok(worker);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        
        [HttpPatch("destroy/{id}")]
        public async Task<ActionResult> DestroyWorker(string id)
        {
            try
            {
                var workerExists = await _context.Workers.AnyAsync(w => w.Id == id);
                if (!workerExists)
                {
                    return NotFound(new { Error = $"Работник с ID: {id} не найден" });
                }

                var employmentHistory = await _context.EmploymentHistories
                    .Where(e => e.WorkerId == id && e.DepartureDate == null)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefaultAsync();
                if (employmentHistory == null)
                {
                    return NotFound(new { Error = $"Активная занятость для работника {id} не найдена" });
                }

                employmentHistory.DepartureDate = DateOnly.FromDateTime(DateTime.Today);

                await _context.SaveChangesAsync();
                return Ok(new { Success = $"Работник с ID: {id} устранен" });
            }
            catch (DbException ex)
            {
                return BadRequest(new { Error = $"Произошла ошибка при соединении с базой данных: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddWorker([FromBody] WorkerDto workerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Error = ModelState });
                }

                // Age
                var ageErrors = await _validationService.ValidateWorkerAge(workerDto.DateOfBirth);
                if (ageErrors.Any())
                {
                    return BadRequest(new { Errors = ageErrors });
                }

                // Conflicts
                var conflicts = await _validationService.ValidateWorkerConflictsAsync(
                    workerDto.Id,
                    workerDto.Phone,
                    workerDto.Email
                );
                if (conflicts.Any())
                {
                    return Conflict(new { Errors = conflicts });
                }

                // Dependencies
                var errors = await _validationService.ValidateWorkerDependenciesAsync(
                    workerDto.PositionId,
                    workerDto.ArrivedAt,
                    string.Empty
                );
                if (errors.Any())
                {
                    return NotFound(new { Errors = errors });
                }

                var worker = new Worker
                {
                    Id = workerDto.Id.Trim(),
                    Name = workerDto.Name.Trim(),
                    Surname = workerDto.Surname.Trim(),
                    Patronymic = workerDto.Patronymic != null ? workerDto.Patronymic.Trim() : null,
                    Gender = workerDto.Gender.Trim(),
                    DateOfBirth = workerDto.DateOfBirth,
                    Phone = workerDto.Phone.Trim(),
                    Email = workerDto.Email.Trim(),
                    Photo = workerDto.Photo != null ? workerDto.Photo : null
                };

                var employmentHistory = new EmploymentHistory
                {
                    WorkerId = workerDto.Id,
                    DateOfArrival = DateOnly.FromDateTime(DateTime.Today),
                    ArrivedAt = workerDto.ArrivedAt.Trim(),
                    PositionId = workerDto.PositionId
                };

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Workers.Add(worker);
                    await _context.SaveChangesAsync();

                    _context.EmploymentHistories.Add(employmentHistory);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
               
                return Ok(new { Success = "Работник добавлен" });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { Error = $"Ошибка при сохранении в базу данных: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateWorker([FromBody] WorkerDto workerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Error = ModelState });
                }

                // Age
                var ageErrors = await _validationService.ValidateWorkerAge(workerDto.DateOfBirth);
                if (ageErrors.Any()) 
                {
                    return BadRequest(new { Errors = ageErrors });
                }

                // Conflicts
                var conflicts = await _validationService.ValidateWorkerConflictsAsync(
                    string.Empty,
                    workerDto.Phone,
                    workerDto.Email,
                    workerDto.Id
                );
                if (conflicts.Any())
                {
                    return Conflict(new { Errors = conflicts });
                }

                // Dependencies
                var errors = await _validationService.ValidateWorkerDependenciesAsync(
                    workerDto.PositionId,
                    workerDto.ArrivedAt,
                    workerDto.Id
                );
                if (errors.Any())
                {
                    return NotFound(new { Errors = errors });
                }

                var worker = await _context.Workers
                    .Include(w => w.EmploymentHistories)
                    .FirstOrDefaultAsync(w => w.Id == workerDto.Id);

                var lastHistory = worker.EmploymentHistories
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefault();

                if (lastHistory == null)
                {
                    return BadRequest(new { Error = "У сотрудника нет записей в истории трудоустройства" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // update
                    worker.Name = workerDto.Name;
                    worker.Surname = workerDto.Surname;
                    worker.Patronymic = workerDto.Patronymic != null ? workerDto.Patronymic : null;
                    worker.Gender = workerDto.Gender;
                    worker.DateOfBirth = workerDto.DateOfBirth;
                    worker.Phone = workerDto.Phone;
                    worker.Email = workerDto.Email;
                    worker.Photo = workerDto.Photo != null ? workerDto.Photo : null;

                    bool positionChanged = workerDto.PositionId != lastHistory.PositionId;
                    bool unitChanged = workerDto.ArrivedAt != lastHistory.ArrivedAt;

                    if (positionChanged || unitChanged)
                    {
                        var today = DateOnly.FromDateTime(DateTime.Today);

                        // Closing the current entry
                        lastHistory.DepartureDate = today;
                        lastHistory.LeftFor = workerDto.ArrivedAt;

                        // Creating a new entry
                        var newHistory = new EmploymentHistory
                        {
                            WorkerId = worker.Id,
                            DateOfArrival = today,
                            ArrivedAt = workerDto.ArrivedAt,
                            PositionId = workerDto.PositionId
                        };

                        _context.EmploymentHistories.Add(newHistory);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { Success = "Данные о работнике изменены" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { Error = $"Ошибка при обновлении данных: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("by-unit/{unitId}")]
        public async Task<ActionResult<IEnumerable<WorkerWithUnitDto>>> GetWorkersByUnit(string unitId)
        {
            try
            {
                var workers = await _context.Workers
                    .OrderBy(w => w.Id)
                    .Include(w => w.EmploymentHistories)
                        .ThenInclude(e => e.Position)
                    .Include(w => w.EmploymentHistories)
                        .ThenInclude(e => e.ArrivedAtNavigation)
                    .Where(w => w.EmploymentHistories
                        .OrderByDescending(e => e.Id)
                        .Take(1)
                        .Any(e => e.ArrivedAt == unitId))
                    .Select(w => new WorkerWithUnitDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Surname = w.Surname,
                        Patronymic = w.Patronymic,
                        Gender = w.Gender,
                        DateOfBirth = w.DateOfBirth,
                        Phone = w.Phone,
                        Email = w.Email,
                        Photo = w.Photo,

                        PositionName = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.Position != null ? e.Position.Name : null)
                            .FirstOrDefault() ?? "Не указана",
                        StructuralUnitName = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.ArrivedAtNavigation != null ? e.ArrivedAtNavigation.Name : null)
                            .FirstOrDefault() ?? "Не указано",
                        DateOfArrival = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.DateOfArrival)
                            .FirstOrDefault()
                            .GetValueOrDefault(),
                        DepartureDate = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.DepartureDate)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(workers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("by-unit-with-children/{unitId}")]
        public async Task<ActionResult<IEnumerable<WorkerWithUnitDto>>> GetWorkersByUnitWithChildren(string unitId)
        {
            try
            {
                // Получаем все дочерние ID
                var childIds = await GetChildUnitIdsAsync(unitId);
                var allUnitIds = new List<string> { unitId };
                allUnitIds.AddRange(childIds);

                var workers = await _context.Workers
                    .Include(w => w.EmploymentHistories)
                        .ThenInclude(e => e.Position)
                    .Include(w => w.EmploymentHistories)
                        .ThenInclude(e => e.ArrivedAtNavigation)
                    .Where(w => w.EmploymentHistories
                        .OrderByDescending(e => e.Id)
                        .Take(1)
                        .Any(e => allUnitIds.Contains(e.ArrivedAt)))
                    .Select(w => new WorkerWithUnitDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Surname = w.Surname,
                        Patronymic = w.Patronymic,
                        Gender = w.Gender,
                        DateOfBirth = w.DateOfBirth,
                        Phone = w.Phone,
                        Email = w.Email,
                        Photo = w.Photo,
                        PositionName = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.Position.Name)
                            .FirstOrDefault() ?? "Не указана",
                        StructuralUnitName = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.ArrivedAtNavigation.Name)
                            .FirstOrDefault() ?? "Не указано",
                        DateOfArrival = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.DateOfArrival)
                            .FirstOrDefault()
                            .GetValueOrDefault(),
                        DepartureDate = w.EmploymentHistories
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.DepartureDate)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(workers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        #region Methods

        private async Task<List<string>> GetChildUnitIdsAsync(string parentId)
        {
            var result = new List<string>();
            var children = await _context.StructuralUnits
                .Where(u => u.ParentId == parentId && u.LiquidationDate == null)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var childId in children)
            {
                result.Add(childId);
                result.AddRange(await GetChildUnitIdsAsync(childId));
            }

            return result;
        }

        /// <summary>
        /// Базовый запрос для активных сотрудников
        /// </summary>
        private IQueryable<Worker> GetActiveWorkersQuery()
        {
            return _context.Workers
                .Where(w => !w.EmploymentHistories
                    .OrderByDescending(e => e.Id)
                    .Take(1)
                    .Any(e => e.DepartureDate != null && e.LeftFor == null));
        }

        /// <summary>
        /// Проекция Worker -> WorkerWithLatestHistoryDto
        /// </summary>
        private IQueryable<WorkerWithLatestHistoryDto> ProjectToWorkerWithLatestHistory(IQueryable<Worker> query)
        {
            return query.Select(w => new WorkerWithLatestHistoryDto
            {
                Id = w.Id,
                Name = w.Name,
                Surname = w.Surname,
                Patronymic = w.Patronymic,
                Gender = w.Gender,
                DateOfBirth = w.DateOfBirth,
                Phone = w.Phone,
                Email = w.Email,
                Photo = w.Photo,

                LatestEmployment = w.EmploymentHistories
                    .OrderByDescending(e => e.Id)
                    .Select(e => new LatestEmploymentDto
                    {
                        Id = e.Id,
                        DateOfArrival = e.DateOfArrival,
                        DepartureDate = e.DepartureDate,
                        ArrivedAt = e.ArrivedAt,
                        LeftFor = e.LeftFor,
                        PositionId = e.PositionId,
                        PositionName = e.Position.Name,
                        ArrivedAtName = e.ArrivedAtNavigation != null ? e.ArrivedAtNavigation.Name : null,
                        LeftForName = e.LeftForNavigation != null ? e.LeftForNavigation.Name : null
                    })
                    .FirstOrDefault()
            });
        }

        private async Task<List<WorkerWithLatestHistoryDto>> GetActiveWorkersAsync()
        {
            var query = GetActiveWorkersQuery().OrderBy(w => w.Id);
            return await ProjectToWorkerWithLatestHistory(query).ToListAsync();
        }

        private async Task<WorkerWithLatestHistoryDto?> GetActiveWorkerByIdAsync(string id)
        {
            var query = GetActiveWorkersQuery().Where(w => w.Id == id);
            return await ProjectToWorkerWithLatestHistory(query).FirstOrDefaultAsync();
        }

        #endregion
    }
}
