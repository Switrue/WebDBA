using System.ComponentModel.DataAnnotations;

namespace WebDBA.Models.DTOs.StructuralUnits
{
    public class CreateStructuralUnitDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Код подразделения обязателен")]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "Код должен содержать от 1 до 4 символов")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Код может содержать только буквы и цифры")]
        [Display(Name = "Код подразделения")]
        public string Id { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Название обязательно")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Название должно содержать от 1 до 200 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; } = null!;

        [StringLength(15, ErrorMessage = "Аббревиатура не может превышать 15 символов")]
        [Display(Name = "Аббревиатура")]
        public string? Abbreviation { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата ликвидации")]
        public DateOnly? LiquidationDate { get; set; }

        [StringLength(4, ErrorMessage = "ID родителя не может превышать 4 символа")]
        [Display(Name = "Родительское подразделение")]
        public string? ParentId { get; set; }

        [Required(ErrorMessage = "Путь предков обязателен")]
        [MinLength(1, ErrorMessage = "Путь предков должен содержать как минимум один элемент")]
        [MaxLength(100, ErrorMessage = "Путь предков не может содержать более 100 элементов")]
        [Display(Name = "Путь предков")]
        public List<string> Ancestors { get; set; } = new List<string>();
    }
}
