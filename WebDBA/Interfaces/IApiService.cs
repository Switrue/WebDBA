using Microsoft.AspNetCore.Mvc.Rendering;
using WebDBA.Models.DTOs.StructuralUnits;
using WebDBA.Models.DTOs.Workers;

namespace WebDBA.Interfaces
{
    public interface IApiService
    {
        // Units
        Task<List<StructuralUnitTreeDto>> GetStructuralUnitsTreeAsync();
        Task<List<SelectListItem>> GetStructuralUnitsSelectListAsync();
        Task<StructuralUnitDto?> GetStructuralUnitByIdAsync(string id);
        Task<List<string>> GetAncestorsPathAsync(string parentId);
        Task<(bool Success, string? ErrorMessage)> CreateStructuralUnitAsync(CreateStructuralUnitDto dto);

        // Workers
        Task<List<WorkerWithUnitDto>> GetWorkersByUnitAsync(string unitId);
        Task<WorkerWithLatestHistoryDto?> GetWorkerByIdAsync(string id);
        Task<List<SelectListItem>> GetPositionsSelectListAsync();
        Task<(bool Success, string? ErrorMessage)> CreateWorkerAsync(CreateWorkerDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateWorkerAsync(string id, UpdateWorkerDto dto);
        Task<(bool Success, string? ErrorMessage)> DismissWorkerAsync(string id);
    }
}
