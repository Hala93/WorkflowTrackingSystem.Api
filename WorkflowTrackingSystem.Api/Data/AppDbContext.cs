using Microsoft.EntityFrameworkCore;
using WorkflowTrackingSystem.Api.Models;

namespace WorkflowTrackingSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<ProcessInstance> ProcessInstances { get; set; }
        public DbSet<ProcessStepInstance> ProcessStepInstances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Workflow>()
                .HasMany(w => w.Steps)
                .WithOne(ws => ws.Workflow)
                .HasForeignKey(ws => ws.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete workflow steps

            modelBuilder.Entity<ProcessInstance>()
                .HasMany(pi => pi.StepInstances)
                .WithOne(psi => psi.ProcessInstance)
                .HasForeignKey(psi => psi.ProcessInstanceId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete process step instances

            // Ensure unique combination for workflow steps
            modelBuilder.Entity<WorkflowStep>()
                .HasIndex(ws => new { ws.WorkflowId, ws.StepName })
                .IsUnique();

            // Example of seeding initial data (optional)
            // modelBuilder.Entity<Workflow>().HasData(
            //     new Workflow { Id = Guid.Parse("..."), Name = "Initial Workflow", Description = "..." }
            // );
        }
    }
}