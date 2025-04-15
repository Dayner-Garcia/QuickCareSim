using Microsoft.EntityFrameworkCore;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Infrastructure.Identity.Entities;

namespace QuickCareSim.Infrastructure.Persistance.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }
        public DbSet<SimulationPerformance> SimulationPerformances { get; set; }
        public DbSet<SimulationRun> SimulationRuns { get; set; }
        public DbSet<AttentionLog> AttentionLogs { get; set; }
        public DbSet<UrgencyWaitMetric> UrgencyWaitMetrics { get; set; }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Identity");
            modelBuilder.Entity<ApplicationUser>().Metadata.SetIsTableExcludedFromMigrations(true);
            #endregion

            #region RelationShiops
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.AttendedPatients)
                .HasForeignKey(p => p.DoctorId)
                .HasPrincipalKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Doctor>().HasKey(d => d.UserId);
            modelBuilder.Entity<Patient>().HasKey(p => p.UserId);

            #endregion

            #region Indexs
            modelBuilder.Entity<Doctor>().HasIndex(d => d.UserId);
            modelBuilder.Entity<Patient>().HasIndex(p => p.UserId);

            #endregion
        }
        #endregion
    }
}
