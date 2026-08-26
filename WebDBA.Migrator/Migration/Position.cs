namespace WebDBA.Migrator.Migration;

public partial class Position
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? StructuralUnitId { get; set; }

    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();

    public virtual StructuralUnit? StructuralUnit { get; set; }
}
