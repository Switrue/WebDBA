using Microsoft.AspNetCore.Mvc.Rendering;
using WebDBA.Interfaces;
using WebDBA.Models.DTOs.Positions;
using WebDBA.Models.DTOs.StructuralUnits;
using WebDBA.Models.DTOs.Workers;

namespace WebDBA.Services
{
    public class ApiService : BaseApiService, IApiService
    {
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
            : base(httpClient, logger)
        {
        }

        /// <summary>
        /// GET methods
        /// </summary>
        public async Task<List<StructuralUnitTreeDto>> GetStructuralUnitsTreeAsync()
        {
            var (success, data, error) = await GetAsync<List<StructuralUnitTreeDto>>("api/structuralunits/tree");

            if (!success)
            {
                _logger.LogError("Ошибка получения дерева подразделений: {Error}", error);
                return new List<StructuralUnitTreeDto>();
            }

            return data ?? new List<StructuralUnitTreeDto>();
        }

        public async Task<StructuralUnitDto?> GetStructuralUnitByIdAsync(string id)
        {
            var (success, data, error) = await GetAsync<StructuralUnitDto>($"api/structuralunits/get/{id}");

            if (!success)
            {
                _logger.LogError("Ошибка получения подразделения {Id}: {Error}", id, error);
                return null;
            }

            return data;
        }

        public async Task<List<WorkerWithUnitDto>> GetWorkersByUnitAsync(string unitId)
        {
            var (success, data, error) = await GetAsync<List<WorkerWithUnitDto>>($"api/workers/by-unit/{unitId}");

            if (!success)
            {
                _logger.LogError("Ошибка получения сотрудников для {UnitId}: {Error}", unitId, error);
                return new List<WorkerWithUnitDto>();
            }

            return data ?? new List<WorkerWithUnitDto>();
        }

        public async Task<WorkerWithLatestHistoryDto?> GetWorkerByIdAsync(string id)
        {
            var (success, data, error) = await GetAsync<WorkerWithLatestHistoryDto>($"api/workers/with-latest-history/{id}");

            if (!success)
            {
                _logger.LogError("Ошибка получения сотрудника {Id}: {Error}", id, error);
                return null;
            }

            return data;
        }

        public async Task<List<SelectListItem>> GetPositionsSelectListAsync()
        {
            var (success, data, error) = await GetAsync<List<PositionDto>>("api/positions/get");

            if (!success || data == null)
            {
                _logger.LogError("Ошибка получения списка должностей: {Error}", error);
                return new List<SelectListItem>();
            }

            return data.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
        }

        public async Task<List<SelectListItem>> GetStructuralUnitsSelectListAsync()
        {
            var (success, data, error) = await GetAsync<List<StructuralUnitDto>>("api/structuralunits/get");

            if (!success || data == null)
            {
                _logger.LogError("Ошибка получения списка подразделений: {Error}", error);
                return new List<SelectListItem>();
            }

            return data.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Id} - {u.Name}"
            }).ToList();
        }

        public async Task<List<string>> GetAncestorsPathAsync(string parentId)
        {
            var (success, data, error) = await GetAsync<List<string>>($"api/structuralunits/ancestors/{parentId}");

            if (!success || data == null)
            {
                _logger.LogError("Ошибка получения списка подразделений: {Error}", error);
                return new List<string>();
            }

            return data;
        }

        /// <summary>
        /// POST methods
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> CreateStructuralUnitAsync(CreateStructuralUnitDto dto)
        {
            return await SendAsync(HttpMethod.Post, "api/structuralunits/add", dto);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateWorkerAsync(CreateWorkerDto dto)
        {
            return await SendAsync(HttpMethod.Post, "api/workers/add", dto);
        }

        /// <summary>
        /// PUT methods
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateWorkerAsync(string id, UpdateWorkerDto dto)
        {
            return await SendAsync(HttpMethod.Put, $"api/workers/update", dto);
        }

        /// <summary>
        /// PATCH methods
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DismissWorkerAsync(string id)
        {
            return await SendAsync(HttpMethod.Patch, $"api/workers/destroy/{id}", null); ;
        }
    }
}
