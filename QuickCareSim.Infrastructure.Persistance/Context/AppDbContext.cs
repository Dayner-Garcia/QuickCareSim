using Microsoft.EntityFrameworkCore;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Infrastructure.Identity.Entities;

namespace QuickCareSim.Infrastructure.Persistance.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #region DbSets

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }
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

            #region Primary Keys

            modelBuilder.Entity<Doctor>().HasKey(d => d.UserId);
            modelBuilder.Entity<Patient>().HasKey(p => p.UserId);

            #endregion

            #region Indexes

            modelBuilder.Entity<Doctor>().HasIndex(d => d.UserId);
            modelBuilder.Entity<Patient>().HasIndex(p => p.UserId);

            #endregion

            #region Relationships

            // Patient -> Doctor
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.AttendedPatients)
                .HasForeignKey(p => p.DoctorId)
                .HasPrincipalKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // PerformanceMetric -> Doctor
            modelBuilder.Entity<PerformanceMetric>()
                .HasOne(pm => pm.Doctor)
                .WithMany(d => d.Metrics)
                .HasForeignKey(pm => pm.DoctorId)
                .HasPrincipalKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PerformanceMetric -> SimulationRun
            modelBuilder.Entity<PerformanceMetric>()
                .HasOne(pm => pm.SimulationRun)
                .WithMany(sr => sr.PerformanceMetrics)
                .HasForeignKey(pm => pm.SimulationRunId)
                .OnDelete(DeleteBehavior.Cascade);

            // UrgencyWaitMetric -> SimulationRun
            modelBuilder.Entity<UrgencyWaitMetric>()
                .HasOne(uwm => uwm.SimulationRun)
                .WithMany(sr => sr.UrgencyWaitMetrics)
                .HasForeignKey(uwm => uwm.SimulationRunId)
                .OnDelete(DeleteBehavior.Cascade);

            // AttentionLog -> SimulationRun
            modelBuilder.Entity<AttentionLog>()
                .HasOne(al => al.SimulationRun)
                .WithMany(sr => sr.AttentionLogs)
                .HasForeignKey(al => al.SimulationRunId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
        #endregion
    }
}