namespace WebDBA.Migrator.Migration;

public partial class EmploymentHistory
{
    public long Id { get; set; }

    public string WorkerId { get; set; } = null!;

    public DateOnly? DateOfArrival { get; set; }

    public DateOnly? DepartureDate { get; set; }

    public string? ArrivedAt { get; set; }

    public string? LeftFor { get; set; }

    public long PositionId { get; set; }

    public virtual StructuralUnit? ArrivedAtNavigation { get; set; }

    public virtual StructuralUnit? LeftForNavigation { get; set; }

    public virtual Position Position { get; set; } = null!;

    public virtual Worker Worker { get; set; } = null!;
}
