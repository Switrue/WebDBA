namespace WebDBA.API.Interfaces
{
    public interface IValidationService
    {
        /// <summary>
        /// WorkersController
        /// </summary>
        Task<bool> ValidateWorkerIdAsync(string workerId);

        Task<bool> ValidateWorkerPhoneAsync(string workerPhone, string? excludeWorkerId = null);
        Task<bool> ValidateWorkerEmailAsync(string workerEmail, string? excludeWorkerId = null);

        Task<Dictionary<string, string>> ValidateWorkerDependenciesAsync(
            long? positionId, 
            string? structuralUnitId,
            string? workerId);

        Task<Dictionary<string, string>> ValidateWorkerConflictsAsync(
            string? workerId,
            string? workerPhone,
            string? workerEmail,
            string? excludeWorkerId = null);

        Task<Dictionary<string, string>> ValidateWorkerAge(DateOnly? dateOfBirth);

        /// <summary>
        /// PositionsController
        /// </summary>
        Task<bool> ValidatePositionIdAsync(long positionId);

        /// <summary>
        /// StructuralUnitsController
        /// </summary
        Task<bool> ValidateStructuralUnitIdAsync(string unitId);

        Task<Dictionary<string, string>> ValidateStructuralUnitConflictsAsync(
            string? structuralUnitId
            );
    }
}
