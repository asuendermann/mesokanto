
using Commons.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;
using DomainModel.Scrum;

namespace DatabaseAccess {
    public class BaseDbContext : DbContext, IDesignTimeDbContextFactory<BaseDbContext> {

        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) {
        }

        public BaseDbContext CreateDbContext(string[] args) {
            var configuration = Commons.Configuration.ConfigurationExtensions.ConfigureFromFile();
            var connectionStringName = configuration.GetAppSetting(Commons.Configuration.ConfigurationExtensions.KeyProjectConnectionString);
            var connectionString = configuration.GetConnectionString(connectionStringName);

            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BaseDbContext(optionsBuilder.Options);
        }

        /// <summary>
        ///     uses EF Fluent API to define data model.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            if (null == modelBuilder) {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .ToTable("Projects")
                .HasKey(p => p.Id);
            modelBuilder.Entity<Project>()
                .HasIndex(w => w.Identifier).IsUnique();

            modelBuilder.Entity<TeamMember>()
                .ToTable("TeamMembers")
                .HasKey(p => p.Id);
            modelBuilder.Entity<TeamMember>()
                .HasIndex(w => w.UserId).IsUnique();
            modelBuilder.Entity<TeamMember>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<TeamMember>(nameof(TeamMember))
                .HasValue<Owner>(nameof(Owner))
                .HasValue<ScrumMaster>(nameof(ScrumMaster));

            modelBuilder.Entity<ProjectTeamMember>()
                .ToTable("ProjectTeamMembers")
                .HasKey(p => p.Id);
            modelBuilder.Entity<ProjectTeamMember>().HasOne<Project>(nameof(Project))
                .WithMany(l => l.ProjectTeamMembers)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
            modelBuilder.Entity<ProjectTeamMember>().HasOne<TeamMember>(nameof(TeamMember))
                .WithMany(l => l.TeamMemberProjects)
                .HasForeignKey(p => p.TeamMemberId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ProjectScrumMaster>()
                .ToTable("ProjectScrumMasters")
                .HasKey(p => p.Id);
            modelBuilder.Entity<ProjectScrumMaster>().HasOne<Project>(nameof(Project))
                .WithMany(l => l.ProjectScrumMasters)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
            modelBuilder.Entity<ProjectScrumMaster>().HasOne<ScrumMaster>(nameof(ScrumMaster))
                .WithMany(l => l.ScrumMasterProjects)
                .HasForeignKey(p => p.ScrumMasterId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ProjectOwner>()
                .ToTable("ProjectOwners")
                .HasKey(p => p.Id);
            modelBuilder.Entity<ProjectOwner>().HasOne<Project>(nameof(Project))
                .WithMany(l => l.ProjectOwners)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
            modelBuilder.Entity<ProjectOwner>().HasOne<Owner>(nameof(Owner))
                .WithMany(l => l.OwnerProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<BacklogItem>()
                .ToTable("BacklogItems")
                .HasKey(p => p.Id);
            modelBuilder.Entity<BacklogItem>().HasOne<Project>(nameof(Project))
                .WithMany(l => l.BacklogItems)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
            modelBuilder.Entity<BacklogItem>().HasOne<ProjectOwner>("Author")
                .WithMany(l => l.BacklogItems)
                .HasForeignKey(p => p.ProjectOwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }

        public override int SaveChanges() {
            this.MarkEntries<int>();
            this.ValidateChanges();

            return base.SaveChanges();
        }
    }
}