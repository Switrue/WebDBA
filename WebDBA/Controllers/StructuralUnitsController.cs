using Microsoft.AspNetCore.Mvc;
using WebDBA.Interfaces;
using WebDBA.Models.DTOs.StructuralUnits;

namespace WebDBA.Controllers
{
    public class StructuralUnitsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<StructuralUnitsController> _logger;

        public StructuralUnitsController(IApiService apiService, ILogger<StructuralUnitsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var units = await _apiService.GetStructuralUnitsSelectListAsync();

            var model = new CreateStructuralUnitDto
            {
                Ancestors = new List<string>()
            };

            ViewBag.UnitsList = units;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStructuralUnitDto dto)
        {
            if (dto == null)
            {
                TempData["Error"] = "Данные не переданы";
                ViewBag.UnitsList = await _apiService.GetStructuralUnitsSelectListAsync();
                return View(new CreateStructuralUnitDto());
            }

            if (dto.Ancestors == null)
            {
                dto.Ancestors = new List<string>();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UnitsList = await _apiService.GetStructuralUnitsSelectListAsync();
                return View(dto);
            }

            try
            {
                var (success, errorMessage) = await _apiService.CreateStructuralUnitAsync(dto);

                if (success)
                {
                    TempData["Success"] = $"Подразделение {dto.Id} - {dto.Name} успешно создано!";
                    return RedirectToAction("Index", "Workers");
                }

                TempData["Error"] = errorMessage ?? "Ошибка при создании подразделения";
                ViewBag.UnitsList = await _apiService.GetStructuralUnitsSelectListAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании подразделения {Id}", dto.Id);
                TempData["Error"] = "Произошла ошибка при создании подразделения";
                ViewBag.UnitsList = await _apiService.GetStructuralUnitsSelectListAsync();
                return View(dto);
            }
        }

        [HttpGet("GetAncestorsPath")]
        public async Task<IActionResult> GetAncestorsPath(string unitId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unitId))
                {
                    return Json(new List<string>());
                }

                var ancestors = await _apiService.GetAncestorsPathAsync(unitId);

                if (ancestors == null || !ancestors.Any())
                {
                    var unit = await _apiService.GetStructuralUnitByIdAsync(unitId);
                    if (unit != null)
                    {
                        return Json(new List<string> { unitId });
                    }
                    return Json(new List<string>());
                }

                return Json(ancestors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения пути к предку для юнита {UnitId}", unitId);
                return Json(new List<string>());
            }
        }
    }
}
