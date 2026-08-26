using Microsoft.AspNetCore.Mvc.Rendering;
using WebDBA.Models.DTOs.Workers;

namespace WebDBA.Models.ViewModels
{
    public class WorkerFormViewModel
    {
        public CreateWorkerDto? CreateDto { get; set; }
        public UpdateWorkerDto? UpdateDto { get; set; }
        public string? WorkerId { get; set; }
        public bool IsEdit => !string.IsNullOrEmpty(WorkerId);

        public List<SelectListItem> Positions { get; set; } = new();
        public List<SelectListItem> StructuralUnits { get; set; } = new();
        public List<SelectListItem> Genders { get; set; } = new()
        {
            new SelectListItem { Value = "Мужской", Text = "Мужской" },
            new SelectListItem { Value = "Женский", Text = "Женский" }
        };
    }
}
