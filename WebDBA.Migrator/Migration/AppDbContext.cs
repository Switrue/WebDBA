using Microsoft.EntityFrameworkCore;

namespace WebDBA.Migrator.Migration;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmploymentHistory> EmploymentHistories { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<StructuralUnit> StructuralUnits { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmploymentHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employment_history_pk");

            entity.ToTable("employment_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArrivedAt)
                .HasMaxLength(4)
                .HasColumnName("arrived_at");
            entity.Property(e => e.DateOfArrival).HasColumnName("date_of_arrival");
            entity.Property(e => e.DepartureDate).HasColumnName("departure_date");
            entity.Property(e => e.LeftFor)
                .HasMaxLength(4)
                .HasColumnName("left_for");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.WorkerId)
                .HasMaxLength(6)
                .HasColumnName("worker_id");

            entity.HasOne(d => d.ArrivedAtNavigation).WithMany(p => p.EmploymentHistoryArrivedAtNavigations)
                .HasForeignKey(d => d.ArrivedAt)
                .HasConstraintName("employment_history_structural_units_fk");

            entity.HasOne(d => d.LeftForNavigation).WithMany(p => p.EmploymentHistoryLeftForNavigations)
                .HasForeignKey(d => d.LeftFor)
                .HasConstraintName("employment_history_structural_units_fk_1");

            entity.HasOne(d => d.Position).WithMany(p => p.EmploymentHistories)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employment_history_positions_fk");

            entity.HasOne(d => d.Worker).WithMany(p => p.EmploymentHistories)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employment_history_workers_fk");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("positions_pk");

            entity.ToTable("positions");

            entity.HasIndex(e => e.Name, "positions_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.StructuralUnitId)
                .HasMaxLength(4)
                .HasColumnName("structural_unit_id");

            entity.HasOne(d => d.StructuralUnit).WithMany(p => p.Positions)
                .HasForeignKey(d => d.StructuralUnitId)
                .HasConstraintName("positions_structural_units_fk");
        });

        modelBuilder.Entity<StructuralUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("structural_unit_pk");

            entity.ToTable("structural_units");

            entity.HasIndex(e => e.Ancestors, "idx_structural_units_ancestors").HasMethod("gin");

            entity.HasIndex(e => e.ParentId, "idx_structural_units_parent_id");

            entity.Property(e => e.Id)
                .HasMaxLength(4)
                .HasColumnName("id");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(15)
                .HasColumnName("abbreviation");
            entity.Property(e => e.Ancestors)
                .HasDefaultValueSql("'{}'::character varying[]")
                .HasColumnType("character varying(4)[]")
                .HasColumnName("ancestors");
            entity.Property(e => e.DateOfCreation).HasColumnName("date_of_creation");
            entity.Property(e => e.LiquidationDate).HasColumnName("liquidation_date");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.ParentId)
                .HasMaxLength(4)
                .HasColumnName("parent_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("structural_units_parent_id_fkey");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("newtable_pk");

            entity.ToTable("workers");

            entity.HasIndex(e => e.Phone, "workers_unique").IsUnique();

            entity.HasIndex(e => e.Email, "workers_unique_1").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .HasColumnName("id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(7)
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(150)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Surname)
                .HasMaxLength(150)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
