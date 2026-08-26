using WebDBA.Models.DTOs.StructuralUnits;
using WebDBA.Models.DTOs.Workers;

namespace WebDBA.Models.ViewModels
{
    public class WorkersIndexViewModel
    {
        public List<StructuralUnitTreeDto> Tree { get; set; } = new();
        public string SelectedUnitId { get; set; } = string.Empty;
        public List<WorkerWithUnitDto> Workers { get; set; } = new();
    }
}
