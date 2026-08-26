namespace WebDBA.API.Models
{
    public class PositionDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? StructuralUnitId { get; set; }
    }
}
