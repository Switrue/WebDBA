using Microsoft.AspNetCore.Mvc;
using WebDBA.Interfaces;
using WebDBA.Models.DTOs.Workers;
using WebDBA.Models.ViewModels;

namespace WebDBA.Controllers
{
    public class WorkersController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<WorkersController> _logger;

        public WorkersController(IApiService apiService, ILogger<WorkersController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? unitId = null)
        {
            try
            {
                var tree = await _apiService.GetStructuralUnitsTreeAsync();

                if (string.IsNullOrEmpty(unitId) && tree.Any())
                {
                    unitId = tree.First().Id;
                }

                var workers = new List<WorkerWithUnitDto>();
                if (!string.IsNullOrEmpty(unitId))
                {
                    workers = await _apiService.GetWorkersByUnitAsync(unitId);
                }

                var viewModel = new WorkersIndexViewModel
                {
                    Tree = tree,
                    SelectedUnitId = unitId ?? string.Empty,
                    Workers = workers
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке страницы");
                TempData["Error"] = "Не удалось загрузить данные";
                return View(new WorkersIndexViewModel());
            }
        }

        [Route("Workers/Profile/{id}")]
        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            var worker = await _apiService.GetWorkerByIdAsync(id);

            if (worker == null) return NotFound();

            return View(worker);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new WorkerFormViewModel
            {
                CreateDto = new CreateWorkerDto(),
                Positions = await _apiService.GetPositionsSelectListAsync(),
                StructuralUnits = await _apiService.GetStructuralUnitsSelectListAsync()
            };
            return View(viewModel);
        }

        [Route("Workers/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var worker = await _apiService.GetWorkerByIdAsync(id);

            if (worker == null)
            {
                return NotFound();
            }

            var viewModel = new WorkerFormViewModel
            {
                WorkerId = id,
                UpdateDto = new UpdateWorkerDto
                {
                    Name = worker.Name,
                    Surname = worker.Surname,
                    Patronymic = worker.Patronymic,
                    Gender = worker.Gender,
                    DateOfBirth = worker.DateOfBirth,
                    Phone = worker.Phone,
                    Email = worker.Email,
                    Photo = worker.Photo,
                    PositionId = worker.LatestEmployment.PositionId,
                    ArrivedAt = worker.LatestEmployment.ArrivedAt
                },
                Positions = await _apiService.GetPositionsSelectListAsync(),
                StructuralUnits = await _apiService.GetStructuralUnitsSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkersByUnit(string unitId)
        {
            try
            {
                var workers = await _apiService.GetWorkersByUnitAsync(unitId);
                return PartialView("_WorkersTablePartial", workers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при загрузке сотрудников для {unitId}");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(string id)
        {
            var (success, errorMessage) = await _apiService.DismissWorkerAsync(id);

            if (success)
            {
                TempData["Success"] = "Сотрудник уволен!";
            }
            else
            {
                TempData["Error"] = errorMessage ?? "Ошибка при увольнении";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkerFormViewModel viewModel, IFormFile? photoFile)
        {
            if (viewModel.CreateDto == null)
            {
                return BadRequest();
            }

            if (photoFile != null && photoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await photoFile.CopyToAsync(memoryStream);
                viewModel.CreateDto.Photo = memoryStream.ToArray();
            }

            if (!ModelState.IsValid)
            {
                viewModel.Positions = await _apiService.GetPositionsSelectListAsync();
                viewModel.StructuralUnits = await _apiService.GetStructuralUnitsSelectListAsync();
                return View(viewModel);
            }

            var (success, errorMessage) = await _apiService.CreateWorkerAsync(viewModel.CreateDto);

            if (success)
            {
                TempData["Success"] = "Сотрудник успешно создан!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = errorMessage ?? "Ошибка при создании сотрудника";
            viewModel.Positions = await _apiService.GetPositionsSelectListAsync();
            viewModel.StructuralUnits = await _apiService.GetStructuralUnitsSelectListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, WorkerFormViewModel viewModel, IFormFile? photoFile)
        {
            if (viewModel.UpdateDto != null)
            {
                viewModel.UpdateDto.Id = id;
            }

            if (viewModel.UpdateDto == null)
            {
                return BadRequest();
            }

            if (photoFile != null && photoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await photoFile.CopyToAsync(memoryStream);
                viewModel.UpdateDto.Photo = memoryStream.ToArray();
            }

            var (success, errorMessage) = await _apiService.UpdateWorkerAsync(id, viewModel.UpdateDto);

            if (success)
            {
                TempData["Success"] = "Данные обновлены!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = errorMessage ?? "Ошибка при обновлении данных";
            viewModel.Positions = await _apiService.GetPositionsSelectListAsync();
            viewModel.StructuralUnits = await _apiService.GetStructuralUnitsSelectListAsync();
            return View(viewModel);
        }
    }
}
