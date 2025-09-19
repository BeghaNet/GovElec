

namespace GovElec.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Models.Demande> Demandes { get; set; } = null!;
    public DbSet<Models.User> Users { get; set; } = null!;
    public DbSet<Models.RefreshToken> RefreshTokens { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Models.Demande>().ToTable("Demandes");
        // modelBuilder.Entity<Models.User>().ToTable("Users");
        modelBuilder.Entity<Demande>()
            .HasOne(d => d.Demandeur)
            .WithMany(u => u.DemandesEmises)
            .HasForeignKey(d => d.DemandeurId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Demande>()
            .HasOne(d => d.Technicien)
            .WithMany(u => u.DemandesAssignees)
            .HasForeignKey(d => d.TechnicienId)
            .OnDelete(DeleteBehavior.Restrict);
          modelBuilder.Entity<RefreshToken>()
            .HasKey(t => t.Token);
		
	}
}
