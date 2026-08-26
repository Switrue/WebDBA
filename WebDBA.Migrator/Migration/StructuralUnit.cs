namespace WebDBA.Migrator.Migration;

public partial class StructuralUnit
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Abbreviation { get; set; }

    public DateOnly DateOfCreation { get; set; }

    public DateOnly? LiquidationDate { get; set; }

    public string? ParentId { get; set; }

    public List<string> Ancestors { get; set; } = null!;

    public virtual ICollection<EmploymentHistory> EmploymentHistoryArrivedAtNavigations { get; set; } = new List<EmploymentHistory>();

    public virtual ICollection<EmploymentHistory> EmploymentHistoryLeftForNavigations { get; set; } = new List<EmploymentHistory>();

    public virtual ICollection<StructuralUnit> InverseParent { get; set; } = new List<StructuralUnit>();

    public virtual StructuralUnit? Parent { get; set; }

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
}
