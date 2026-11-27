using OpsRescue.Models;
using Microsoft.EntityFrameworkCore;

namespace OpsRescue.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OperationsManagerProfile> OperationsManagerProfiles { get; set; }
        public DbSet<OperationsUserProfile> OperationsUserProfiles { get; set; }
        public DbSet<DeliveryPartnerProfile> DeliveryPartnerProfiles { get; set; }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<ImpactBlastRadius> ImpactBlastRadiuses { get; set; }
        public DbSet<ImpactExposureDossier> ImpactExposureDossiers { get; set; }
        public DbSet<ImpactGraphNode> ImpactGraphNodes { get; set; }
        public DbSet<ImpactGraphEdge> ImpactGraphEdges { get; set; }
        public DbSet<IncidentSimulation> IncidentSimulations { get; set; }
        public DbSet<SimulationStrategy> SimulationStrategies { get; set; }
        public DbSet<SlaRiskAssessment> SlaRiskAssessments { get; set; }
        public DbSet<SlaTrendPoint> SlaTrendPoints { get; set; }
        public DbSet<SlaContributingFactor> SlaContributingFactors { get; set; }
        public DbSet<SlaShipmentContext> SlaShipmentContexts { get; set; }
        public DbSet<IncidentTimelineEvent> IncidentTimelineEvents { get; set; }
        public DbSet<RecoveryExecution> RecoveryExecutions { get; set; }
        public DbSet<ManagerApprovalLog> ManagerApprovalLogs { get; set; }
        public DbSet<AccountabilityTrustScore> AccountabilityTrustScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints on users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Operations Manager Profile (1-to-1 + Unique EmployeeId)
            modelBuilder.Entity<OperationsManagerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<OperationsManagerProfile>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.OperationsManagerProfile)
                .WithOne(p => p.User)
                .HasForeignKey<OperationsManagerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Operations User Profile (1-to-1 + Unique EmployeeId)
            modelBuilder.Entity<OperationsUserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<OperationsUserProfile>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.OperationsUserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<OperationsUserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Delivery Partner Profile (1-to-1 + Unique PartnerId)
            modelBuilder.Entity<DeliveryPartnerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<DeliveryPartnerProfile>()
                .HasIndex(p => p.PartnerId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.DeliveryPartnerProfile)
                .WithOne(p => p.User)
                .HasForeignKey<DeliveryPartnerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Incident Alternate Key on IncidentId
            modelBuilder.Entity<Incident>()
                .HasAlternateKey(i => i.IncidentId);

            modelBuilder.Entity<IncidentSimulation>()
                .HasAlternateKey(s => s.IncidentId);

            modelBuilder.Entity<SlaRiskAssessment>()
                .HasAlternateKey(s => s.IncidentId);

            // 1-to-1 Incident -> BlastRadius
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.BlastRadius)
                .WithOne(b => b.Incident)
                .HasPrincipalKey<Incident>(i => i.IncidentId)
                .HasForeignKey<ImpactBlastRadius>(b => b.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> ExposureDossiers
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.ExposureDossiers)
                .WithOne(d => d.Incident)
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> GraphNodes
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.GraphNodes)
                .WithOne(n => n.Incident)
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(n => n.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> GraphEdges
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.GraphEdges)
                .WithOne(e => e.Incident)
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1 Incident -> Simulation
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Simulation)
                .WithOne(s => s.Incident)
                .HasPrincipalKey<Incident>(i => i.IncidentId)
                .HasForeignKey<IncidentSimulation>(s => s.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many IncidentSimulation -> SimulationStrategies
            modelBuilder.Entity<IncidentSimulation>()
                .HasMany(sim => sim.Strategies)
                .WithOne()
                .HasPrincipalKey(sim => sim.IncidentId)
                .HasForeignKey(strat => strat.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1 Incident -> SlaRiskAssessment
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.SlaRiskAssessment)
                .WithOne(s => s.Incident)
                .HasPrincipalKey<Incident>(i => i.IncidentId)
                .HasForeignKey<SlaRiskAssessment>(s => s.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many SlaRiskAssessment -> TrendPoints
            modelBuilder.Entity<SlaRiskAssessment>()
                .HasMany(sla => sla.TrendPoints)
                .WithOne()
                .HasPrincipalKey(sla => sla.IncidentId)
                .HasForeignKey(tp => tp.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many SlaRiskAssessment -> ContributingFactors
            modelBuilder.Entity<SlaRiskAssessment>()
                .HasMany(sla => sla.ContributingFactors)
                .WithOne()
                .HasPrincipalKey(sla => sla.IncidentId)
                .HasForeignKey(cf => cf.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1 SlaRiskAssessment -> ShipmentContext
            modelBuilder.Entity<SlaRiskAssessment>()
                .HasOne(sla => sla.ShipmentContext)
                .WithOne()
                .HasPrincipalKey<SlaRiskAssessment>(sla => sla.IncidentId)
                .HasForeignKey<SlaShipmentContext>(sc => sc.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> TimelineEvents
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.TimelineEvents)
                .WithOne()
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(t => t.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> RecoveryExecutions
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.RecoveryExecutions)
                .WithOne()
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> ApprovalLogs
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.ApprovalLogs)
                .WithOne()
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(a => a.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Incident -> TrustScores
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.TrustScores)
                .WithOne()
                .HasPrincipalKey(i => i.IncidentId)
                .HasForeignKey(t => t.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}