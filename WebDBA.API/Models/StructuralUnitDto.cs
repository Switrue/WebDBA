using System.ComponentModel.DataAnnotations;

namespace WebDBA.API.Models
{
    public class StructuralUnitDto
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(4)]
        public string Id { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 200)]
        public string Name { get; set; } = null!;

        [StringLength(maximumLength: 15)]
        public string? Abbreviation { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? LiquidationDate { get; set; }

        [StringLength(4)]
        public string? ParentId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(1)]
        [MaxLength(100)]
        public List<string> Ancestors { get; set; } = null!;
    }
}
