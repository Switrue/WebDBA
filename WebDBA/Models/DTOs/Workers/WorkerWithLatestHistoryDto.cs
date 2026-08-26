namespace WebDBA.Models.DTOs.Workers
{
    public class WorkerWithLatestHistoryDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string FullName => $"{Surname} {Name} {Patronymic}".Trim();
        public string Gender { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[]? Photo { get; set; }
        public LatestEmploymentDto? LatestEmployment { get; set; }
        public bool IsActive => LatestEmployment?.DepartureDate == null;
    }
}
