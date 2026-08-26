namespace WebDBA.Models.DTOs.Workers
{
    public class LatestEmploymentDto
    {
        public long Id { get; set; }
        public DateOnly DateOfArrival { get; set; }
        public DateOnly? DepartureDate { get; set; }
        public string ArrivedAt { get; set; } = null!;
        public string? LeftFor { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; } = null!;
        public string ArrivedAtName { get; set; } = null!;
        public string? LeftForName { get; set; }
    }
}
