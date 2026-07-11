using Microsoft.EntityFrameworkCore;

namespace CampingAI.Infra.Models.REDARBOR_DB;

public partial class REDARBOR_TTContext : DbContext {
    public REDARBOR_TTContext(DbContextOptions<REDARBOR_TTContext> options)
        : base(options) {
    }

    public virtual DbSet<T_EMPLOYEES> T_EMPLOYEES { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<T_EMPLOYEES>(entity => {
            entity.HasKey(e => e.EMP_IdEmployee);

            entity.Property(e => e.EMP_IdEmployee).ValueGeneratedNever();
            entity.Property(e => e.EMP_CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EMP_Email).HasMaxLength(255);
            entity.Property(e => e.EMP_Fax).HasMaxLength(50);
            entity.Property(e => e.EMP_Name).HasMaxLength(255);
            entity.Property(e => e.EMP_Password).HasMaxLength(255);
            entity.Property(e => e.EMP_Telephone).HasMaxLength(50);
            entity.Property(e => e.EMP_UpdatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EMP_Username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
