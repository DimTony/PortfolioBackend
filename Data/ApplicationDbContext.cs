using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Models;
using System.Text.Json;

namespace PortfolioBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<ProjectDescription> ProjectDescriptions { get; set; } = null!;
        public DbSet<Objective> Objectives { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Project and ProjectDescription
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Desc)
                .WithOne()
                .HasForeignKey<ProjectDescription>("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between ProjectDescription and Objectives
            modelBuilder.Entity<ProjectDescription>()
                .HasMany(pd => pd.Objectives)
                .WithOne()
                .HasForeignKey("ProjectDescriptionId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the list of technologies as JSON
            var options = new JsonSerializerOptions();
            modelBuilder.Entity<Project>()
                .Property(p => p.Technologies)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, options),
                    v => JsonSerializer.Deserialize<List<string>>(v, options) ?? new List<string>()
                );

            // Fix for identity column issues
            modelBuilder.Entity<Project>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<ProjectDescription>()
                .Property(pd => pd.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<Objective>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}