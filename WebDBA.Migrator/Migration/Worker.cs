namespace WebDBA.Migrator.Migration;

public partial class Worker
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}
