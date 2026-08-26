namespace WebDBA.API.Models
{
    public class StructuralUnitSimpleTreeDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public DateOnly DateOfCreation { get; set; }
        public DateOnly? LiquidationDate { get; set; }
        public string? ParentId { get; set; }

        /// <summary>
        /// Активно ли подразделение (не ликвидировано)
        /// </summary>
        public bool IsActive => LiquidationDate == null;

        /// <summary>
        /// Дочерние подразделения
        /// </summary>
        public List<StructuralUnitSimpleTreeDto> Children { get; set; } = new();

        /// <summary>
        /// Есть ли дочерние элементы
        /// </summary>
        public bool HasChildren => Children?.Any() == true;
    }
}
