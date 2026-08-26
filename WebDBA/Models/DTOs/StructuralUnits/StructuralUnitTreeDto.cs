namespace WebDBA.Models.DTOs.StructuralUnits
{
    public class StructuralUnitTreeDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public DateOnly DateOfCreation { get; set; }
        public DateOnly? LiquidationDate { get; set; }
        public string? ParentId { get; set; }
        public bool IsActive => LiquidationDate == null;
        public List<StructuralUnitTreeDto> Children { get; set; } = new();
        public bool HasChildren => Children?.Any() == true;
        public int EmployeeCount { get; set; }
    }
}
