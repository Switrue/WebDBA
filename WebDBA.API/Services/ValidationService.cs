using Microsoft.EntityFrameworkCore;
using WebDBA.API.Interfaces;
using WebDBA.Migrator.Migration;

namespace WebDBA.API.Services
{
    public class ValidationService : IValidationService
    {
        private readonly AppDbContext _context;

        public ValidationService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// WorkerController
        /// </summary>
        public async Task<bool> ValidateWorkerIdAsync(string workerId)
        {
            return await _context.Workers.AnyAsync(w => w.Id == workerId);
        }

        public async Task<bool> ValidateWorkerPhoneAsync(string workerPhone, string? excludeWorkerId = null)
        {
            var query = _context.Workers.Where(w => w.Phone == workerPhone);

            if (!string.IsNullOrEmpty(excludeWorkerId))
            {
                query = query.Where(w => w.Id != excludeWorkerId);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ValidateWorkerEmailAsync(string workerEmail, string? excludeWorkerId = null)
        {
            var query = _context.Workers.Where(w => w.Email == workerEmail);

            if (!string.IsNullOrEmpty(excludeWorkerId))
            {
                query = query.Where(w => w.Id != excludeWorkerId);
            }

            return await query.AnyAsync();
        }

        public async Task<Dictionary<string, string>> ValidateWorkerDependenciesAsync(
            long? positionId,
            string? structuralUnitId,
            string? workerId)
        {
            var errors = new Dictionary<string, string>();

            if (positionId.HasValue && !await ValidatePositionIdAsync(positionId.Value))
            {
                errors.Add("PositionId", $"Должность с ID {positionId} не найдена");
            }

            if (!string.IsNullOrEmpty(structuralUnitId) &&
                !await ValidateStructuralUnitIdAsync(structuralUnitId))
            {
                errors.Add("StructuralUnitId", $"Подразделение с ID {structuralUnitId} не найдено");
            }

            if (!string.IsNullOrEmpty(workerId) &&
                !await ValidateWorkerIdAsync(workerId))
            {
                errors.Add("WorkerId", $"Работник с ID {workerId} не найден");
            }

            return errors;
        }

        public async Task<Dictionary<string, string>> ValidateWorkerConflictsAsync(
             string? workerId,
             string? workerPhone,
             string? workerEmail,
             string? excludeWorkerId = null)
        {
            var errors = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(workerId) &&
                await ValidateWorkerIdAsync(workerId))
            {
                errors.Add("WorkerId", $"ID '{workerId}' уже используется");
            }

            if (!string.IsNullOrEmpty(workerPhone) &&
                await ValidateWorkerPhoneAsync(workerPhone, excludeWorkerId))
            {
                errors.Add("WorkerPhone", $"Телефон '{workerPhone}' уже используется");
            }

            if (!string.IsNullOrEmpty(workerEmail) &&
                await ValidateWorkerEmailAsync(workerEmail, excludeWorkerId)) 
            {
                errors.Add("WorkerEmail", $"Email '{workerEmail}' уже используется");
            }

            return errors;
        }

        public async Task<Dictionary<string, string>> ValidateWorkerAge(DateOnly? dateOfBirth)
        {
            var errors = new Dictionary<string, string>();

            if (!dateOfBirth.HasValue)
            {
                errors.Add("DateOfBirth", "Дата рождения обязательна");
                return errors;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var minBirthDate = today.AddYears(-110);
            var correctBirtDate = today.AddYears(-18);

            if (dateOfBirth.Value < minBirthDate)
            {
                errors.Add("MinDateOfBirth", "Работник не может быть старше 110 лет");
            }

            if (dateOfBirth.Value > correctBirtDate)
            {
                errors.Add("CorrectDateOfBirth", "Работник должен быть старше 18 лет");
            }

            return errors;
        }

        /// <summary>
        /// PositionsController
        /// </summary>
        public async Task<bool> ValidatePositionIdAsync(long positionId)
        {
            return await _context.Positions.AnyAsync(p => p.Id == positionId);
        }

        /// <summary>
        /// StructuralUnitsController
        /// </summary>
        public async Task<bool> ValidateStructuralUnitIdAsync(string unitId)
        {
            return await _context.StructuralUnits.AnyAsync(s => s.Id == unitId);
        }

        public async Task<Dictionary<string, string>> ValidateStructuralUnitConflictsAsync(
            string? structuralUnitId
            )
        {
            var errors = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(structuralUnitId) &&
                await ValidateStructuralUnitIdAsync(structuralUnitId))
            {
                errors.Add("StructuralUnitId", $"ID '{structuralUnitId}' уже используется");
            }

            return errors;
        }
    }
}
