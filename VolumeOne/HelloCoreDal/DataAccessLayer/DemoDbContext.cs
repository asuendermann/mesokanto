using System;
using System.Linq;

using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace HelloCoreDal.DataAccessLayer {
    public class DemoDbContext : BaseDbContext {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) {
        }

        public DbSet<Administrator> Administrators { get; set; }

        public DbSet<ProjectAdministrator> ProjectAdministrators { get; set; }

        public DbSet<MasterAdministrator> MasterAdministrators { get; set; }

        /// <summary>
        ///     uses EF Fluent API to refine data model.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            if (null == modelBuilder) {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            // -------------------------------
            // Configure ProjectAdministrators
            // -------------------------------
            modelBuilder.Entity<Administrator>().ToTable("Administrators")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ProjectAdministrator>(typeof(ProjectAdministrator).Name)
                .HasValue<MasterAdministrator>(typeof(MasterAdministrator).Name);

            modelBuilder.Entity<Administrator>().HasKey(p => p.Id);
            modelBuilder.Entity<Administrator>()
                .HasIndex(w => w.UserIdentityName).IsUnique();
        }

        public void SeedDatabase(string masterUserIdentityName) {
            if (!MasterAdministrators.Any(a => a.UserIdentityName == masterUserIdentityName)) {
                var master = new MasterAdministrator {
                    UserIdentityName = $"{masterUserIdentityName}",
                    Name = "Master User",
                    Email = "master@domain.com",
                    Phone = "1234"
                };

                Administrators.Add(master);
                Log.Information($"Added Master {masterUserIdentityName}");
            } else {
                Log.Information($"Master {masterUserIdentityName} already in database.");
            }

            SaveChanges();
        }
    }
}