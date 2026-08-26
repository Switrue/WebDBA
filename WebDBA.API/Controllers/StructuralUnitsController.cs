using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDBA.API.Interfaces;
using WebDBA.API.Models;
using WebDBA.Migrator.Migration;

namespace WebDBA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StructuralUnitsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidationService _validationService;

        public StructuralUnitsController(AppDbContext context, IValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<StructuralUnitDto>>> GetStructuralUnits()
        {
            try
            {
                var units = await _context.StructuralUnits
                    .OrderBy(x => x.Id)
                    .Select(u => new StructuralUnitDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Abbreviation = u.Abbreviation,
                        LiquidationDate = u.LiquidationDate,
                        ParentId = u.ParentId,
                        Ancestors = u.Ancestors
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(units);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<IEnumerable<StructuralUnitDto>>> GetStructuralUnit(string id)
        {
            try
            {
                var unit = await _context.StructuralUnits
                    .Where(x => x.Id == id)
                    .Select(u => new StructuralUnitDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Abbreviation = u.Abbreviation,
                        LiquidationDate = u.LiquidationDate,
                        ParentId = u.ParentId,
                        Ancestors = u.Ancestors
                    })
                    .FirstOrDefaultAsync();

                if (unit == null)
                {
                    return NotFound(new { Error = $"Подразделение с ID {id} не найдено" });
                }

                return Ok(unit);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<StructuralUnitSimpleTreeDto>>> GetStructuralUnitsTree()
        {
            try
            {
                var allUnits = await _context.StructuralUnits
                    .Select(s => new StructuralUnitSimpleTreeDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Abbreviation = s.Abbreviation,
                        DateOfCreation = s.DateOfCreation,
                        LiquidationDate = s.LiquidationDate,
                        ParentId = s.ParentId
                    })
                    .ToListAsync();

                var unitsDict = allUnits.ToDictionary(u => u.Id);

                var rootUnits = new List<StructuralUnitSimpleTreeDto>();

                foreach (var unit in allUnits)
                {
                    if (string.IsNullOrEmpty(unit.ParentId))
                    {
                        rootUnits.Add(unit);
                    }
                    else if (unitsDict.TryGetValue(unit.ParentId, out var parent))
                    {
                        parent.Children.Add(unit);
                    }
                }

                return Ok(rootUnits);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("destroy/{id}")]
        public async Task<ActionResult> DestroyStructuralUnit(string id)
        {
            try
            {
                var structuralUnit = await _context.StructuralUnits
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();

                if (structuralUnit == null)
                {
                    return NotFound(new { Error = $"Подразделение с ID {id} не найдено" });
                }

                if (structuralUnit.LiquidationDate != null)
                {
                    return BadRequest(new { Error = $"Подразделение {id} уже ликвидировано" });
                }

                structuralUnit.LiquidationDate = DateOnly.FromDateTime(DateTime.Today);
                await _context.SaveChangesAsync();

                return Ok(new { Success = $"Подразделение {id} ликвидировано" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddStructuralUnit([FromBody] StructuralUnitDto structuralUnitDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Error = ModelState });
                }

                var conflicts = await _validationService.ValidateStructuralUnitConflictsAsync(
                    structuralUnitDto.Id
                    );
                if (conflicts.Any())
                {
                    return Conflict(new { Errors = conflicts });
                }

               var structuralUnit = new StructuralUnit
                {
                    Id = structuralUnitDto.Id,
                    Name = structuralUnitDto.Name,
                    Abbreviation = structuralUnitDto.Abbreviation != null ? structuralUnitDto.Abbreviation : null,
                    DateOfCreation = DateOnly.FromDateTime(DateTime.Today),
                    LiquidationDate = structuralUnitDto.LiquidationDate != null ? structuralUnitDto.LiquidationDate : null,
                    ParentId = structuralUnitDto.ParentId != null ? structuralUnitDto.ParentId : null,
                    Ancestors = structuralUnitDto.Ancestors
                };

                _context.StructuralUnits.Add(structuralUnit);
                await _context.SaveChangesAsync();

                return Ok(new { Success = "Подразделение добавлено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateStructuralUnit([FromBody] StructuralUnitDto structuralUnitDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Error = ModelState });
                }

                var unit = await _context.StructuralUnits
                    .FirstOrDefaultAsync(u => u.Id == structuralUnitDto.Id);

                if (unit == null)
                {
                    return NotFound(new { Error = $"Подразделение с ID {structuralUnitDto.Id} не найдено" });
                }

                var existingIds = await _context.StructuralUnits
                    .Where(u => structuralUnitDto.Ancestors.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();

                var missingIds = structuralUnitDto.Ancestors
                    .Except(existingIds)
                    .ToList();

                if (missingIds.Any())
                {
                    return NotFound($"Путь структурного подразделения не был найден: {string.Join(", ", missingIds)}");
                }

                // Update
                unit.Name = structuralUnitDto.Name;
                unit.Ancestors = structuralUnitDto.Ancestors;
                unit.ParentId = structuralUnitDto.ParentId;
                unit.Ancestors = structuralUnitDto.Ancestors.ToList();

                await _context.SaveChangesAsync();

                return Ok(new { Success = "Структурное подразделение обновлено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("ancestors/{id}")]
        public async Task<ActionResult<List<string>>> GetAncestorsPath(string id)
        {
            try
            {
                var currentUnit = await _context.StructuralUnits
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (currentUnit == null)
                {
                    return NotFound(new { Error = $"Подразделение с ID '{id}' не найдено" });
                }

                var ancestors = new List<string>();
                var current = currentUnit;

                while (current != null && !string.IsNullOrEmpty(current.ParentId))
                {
                    ancestors.Insert(0, current.ParentId);

                    current = await _context.StructuralUnits
                        .FirstOrDefaultAsync(u => u.Id == current.ParentId);

                    if (ancestors.Count > 100)
                    {
                        break;
                    }
                }

                ancestors.Add(id);

                return Ok(ancestors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
