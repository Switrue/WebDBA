using System.ComponentModel.DataAnnotations;

namespace WebDBA.Models.DTOs.Workers
{
    public class UpdateWorkerDto
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(150, ErrorMessage = "Имя должно содержать не более 150 символов")]
        [Display(Name = "Имя")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(150, ErrorMessage = "Фамилия должна содержать не более 150 символов")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Отчество должно содержать не более 150 символов")]
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Пол обязателен")]
        [StringLength(7, ErrorMessage = "Пол должен содержать не более 7 символов")]
        [Display(Name = "Пол")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Телефон обязателен")]
        [StringLength(15, ErrorMessage = "Телефон должен содержать не более 15 символов")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Email обязателен")]
        [StringLength(100, ErrorMessage = "Email должен содержать не более 100 символов")]
        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Фото")]
        public byte[]? Photo { get; set; }

        [Required(ErrorMessage = "Должность обязательна")]
        [Display(Name = "Должность")]
        public long PositionId { get; set; }

        [Required(ErrorMessage = "Подразделение обязательно")]
        [StringLength(4, ErrorMessage = "ID подразделения должен содержать 4 символа")]
        [Display(Name = "Подразделение")]
        public string ArrivedAt { get; set; } = null!;
    }
}
