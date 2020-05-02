
using Commons.Configuration;
using DomainModel.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DatabaseAccess {
    public class BaseDbContext : DbContext, IDesignTimeDbContextFactory<BaseDbContext> {

        public static IEnumerable<Type> TypesWithConstraint = new[] { typeof(Administrator) };
        public static Dictionary<Type, MethodInfo> CreateComparers = new Dictionary<Type, MethodInfo>();
        public static Dictionary<Type, MethodInfo> UpdateComparers = new Dictionary<Type, MethodInfo>();

        static BaseDbContext() {
            var thisType = typeof(BaseDbContext);
            foreach ( var typeWithConstraint in TypesWithConstraint) {
                var updateMethod = thisType.GetMethod("UpdateComparer",
                  BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Standard,
                  new[] { typeWithConstraint, typeWithConstraint }, null);
                if (null != updateMethod && typeof(bool).IsAssignableFrom(updateMethod.ReturnType) 
                    && !UpdateComparers.ContainsKey(typeWithConstraint) ) {
                    UpdateComparers.Add(typeWithConstraint, updateMethod);
                }
                var createMethod = thisType.GetMethod("CreateComparer",
                    BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Standard,
                    new[] { typeWithConstraint, typeWithConstraint }, null);
                if (null != createMethod && typeof(bool).IsAssignableFrom(createMethod.ReturnType)
                    && !CreateComparers.ContainsKey(typeWithConstraint)) {
                    CreateComparers.Add(typeWithConstraint, createMethod);
                }
            }
        }

        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) {
        }

        public BaseDbContext CreateDbContext(string[] args) {
            var configuration = ConfigurationTk.ConfigureFromFile();
            var connectionStringName = configuration.GetAppSetting(ConfigurationTk.ProjectConnectionString);
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

            modelBuilder.Entity<Administrator>().ToTable("Administrators")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ProjectAdministrator>(typeof(ProjectAdministrator).Name)
                .HasValue<MasterAdministrator>(typeof(MasterAdministrator).Name);

            modelBuilder.Entity<Administrator>().HasKey(p => p.Id);
            modelBuilder.Entity<Administrator>()
                .HasIndex(w => w.UserIdentityName).IsUnique();
        }

        public override int SaveChanges() {
            this.MarkEntries<int>();
            this.ValidateChanges();

            return base.SaveChanges();
        }

        /// <summary>
        ///     function that checks for duplicates before Create is performed.
        /// </summary>
        /// <param name="a1">first entity to be checked</param>
        /// <param name="a2">second entity to be checked</param>
        /// <returns>true if another entry already uses the specified key, false otherwise</returns>
        public static bool CreateComparer(Administrator a1, Administrator a2) {
            return a1.UserIdentityName == a2.UserIdentityName;
        }

        /// <summary>
        ///     function that checks for duplicates before Update is performed.
        /// </summary>
        /// <param name="a1">first entity to be checked</param>
        /// <param name="a2">second entity to be checked</param>
        /// <returns>true if another entry already uses the specified key, false otherwise</returns>
        public static bool UpdateComparer(Administrator a1, Administrator a2) {
            return a1.UserIdentityName == a2.UserIdentityName && a1.Id != a2.Id;
        }

    }
}