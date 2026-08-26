using System.ComponentModel.DataAnnotations;

namespace WebDBA.API.Models
{
    public class WorkerDto
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(6)]
        public string Id { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 150)]
        public string Name { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 150)]
        public string Surname { get; set; } = null!;

        [StringLength(maximumLength: 150)]
        public string? Patronymic { get; set; }

        [StringLength(maximumLength: 7)]
        [Required(AllowEmptyStrings = false)]
        public string Gender { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 15)]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public byte[]? Photo { get; set; }

        [Required]
        public long PositionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(4)]
        public string ArrivedAt { get; set; } = null!;
    }
}
