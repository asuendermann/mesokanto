using System;
using System.Linq;

using HelloCoreDal.DataAccessLayer;
using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace HelloCoreTest.DAL {
    public class TestDalAdministrators : BaseDataLayerTest {
        [Test]
        public void TestChangeType() {
            var expProjectAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expProjectAdminAdd));

            var projectAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expProjectAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTypePerHierarchy<ProjectAdministrator>(projectAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>
                (expProjectAdminAdd, projectAdmin);

            ChangeTypeOfEntry<Administrator, MasterAdministrator, int>(projectAdmin);

            var changedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expProjectAdminAdd.UserIdentityName);
            Assert.NotNull(changedAdmin);
            Assert.AreEqual(typeof(MasterAdministrator).Name, changedAdmin.Discriminator);
            Assert.AreEqual(typeof(MasterAdministrator), changedAdmin.GetType());
        }

        [Test]
        public void TestProjectAdministratorCrud1() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expAdminAdd));

            var projectAdmin = DbContext.ProjectAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTypePerHierarchy<ProjectAdministrator>(projectAdmin);
            AssertAdministrator<ProjectAdministrator, ProjectAdministrator, int>(expAdminAdd, projectAdmin);

            ModifyAdministrator<ProjectAdministrator, int>(projectAdmin, expAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.ProjectAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<ProjectAdministrator, ProjectAdministrator, int>(expAdminUpdate, modifiedAdmin);

            DbContext.ProjectAdministrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestProjectAdministratorCrud2() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expAdminAdd));

            var addedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(addedAdmin);
            AssertBaseCreated(addedAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>(expAdminAdd, addedAdmin);

            ModifyAdministrator<Administrator, int>(addedAdmin, expAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            Assert.True(modifiedAdmin is ProjectAdministrator);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>(expAdminUpdate, modifiedAdmin);

            DbContext.Administrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestMasterAdministratorCrud1() {
            var expMasterAdminAdd = CreateAdministrator<MasterAdministrator, int>();
            var expMasterAdminUpdate = CreateAdministrator<MasterAdministrator, int>(2);
            Assert.True(TryAddEntity<MasterAdministrator, int>(expMasterAdminAdd));

            var masterAdmin = DbContext.MasterAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminAdd.UserIdentityName);
            Assert.NotNull(masterAdmin);
            AssertBaseCreated(masterAdmin);
            AssertTypePerHierarchy<MasterAdministrator>(masterAdmin);
            AssertAdministrator<MasterAdministrator, MasterAdministrator, int>(expMasterAdminAdd, masterAdmin);

            ModifyAdministrator<MasterAdministrator, int>(masterAdmin, expMasterAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.MasterAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<MasterAdministrator, MasterAdministrator, int>(expMasterAdminUpdate, modifiedAdmin);

            DbContext.MasterAdministrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestMasterAdministratorCrud2() {
            var expMasterAdminAdd = CreateAdministrator<MasterAdministrator, int>();
            var expMasterAdminUpdate = CreateAdministrator<MasterAdministrator, int>(2);
            Assert.True(TryAddEntity<MasterAdministrator, int>(expMasterAdminAdd));

            var masterAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminAdd.UserIdentityName);
            Assert.NotNull(masterAdmin);
            AssertBaseCreated(masterAdmin);
            AssertAdministrator<MasterAdministrator, Administrator, int>(expMasterAdminAdd, masterAdmin);

            ModifyAdministrator<Administrator, int>(masterAdmin, expMasterAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            Assert.True(modifiedAdmin is MasterAdministrator);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<MasterAdministrator, Administrator, int>(expMasterAdminUpdate, modifiedAdmin);

            DbContext.Administrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestProjectAdministratorDuplicateIndex1() {
            var projectAdmin1 = CreateAdministrator<ProjectAdministrator, int>();
            TryAddEntity<ProjectAdministrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
                TryAddEntity<ProjectAdministrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestProjectAdministratorDuplicateIndex2() {
            var projectAdmin1 = CreateAdministrator<ProjectAdministrator, int>();
            TryAddEntity<Administrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
                TryAddEntity<Administrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestMasterAdministratorDuplicateIndex1() {
            var projectAdmin1 = CreateAdministrator<MasterAdministrator, int>();
            TryAddEntity<MasterAdministrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<MasterAdministrator, int>();
                TryAddEntity<MasterAdministrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestMasterAdministratorDuplicateIndex2() {
            var projectAdmin1 = CreateAdministrator<MasterAdministrator, int>();
            TryAddEntity<Administrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<MasterAdministrator, int>();
                TryAddEntity<Administrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestProjectAdministratorUserIdentityNameAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                invokeException);
        }

        [Test]
        public void TestProjectAdministratorUserIdentityNameUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                invokeException);
        }

        [Test]
        public void TestProjectAdministratorNameAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorNameUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorEmailAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorEmailUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));
            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorPhoneAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 32, invokeException);
        }

        [Test]
        public void TestProjectAdministratorPhoneUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 32, invokeException);
        }


        /// <summary>
        /// Diese Testfall ist bewusst static um keine Seiteneffekte aus anderen Tests zu haben - sozusage ein Test in Quarantäne.
        /// </summary>
        [Test]
        public static void TestChangeTracking() {
            // create a new DbContext
            using var context = DependencyInjector.GetServiceProvider().GetService(typeof(DemoDbContext)) as DemoDbContext;
            if (null == context) {
                throw new ArgumentNullException(typeof(DemoDbContext).Name);
            }
            context.Database.EnsureCreated();

            // Change Tracker in a new created DbContext is empty
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());

            // Add a single entity by setting its state to Added should add it to Tracking 
            var administrator = CreateAdministrator<Administrator,int>();
            context.Entry(administrator).State = EntityState.Added;
            Assert.AreEqual(1, context.ChangeTracker.Entries().Count());

            // Saving the changes keeps entry tracked but sets its state to Unchanged
            Assert.AreEqual(1, context.SaveChanges());
            Assert.AreEqual(1, context.ChangeTracker.Entries().Count());
            Assert.AreEqual(EntityState.Unchanged, context.Entry(administrator).State);

            // a change to the tracked entry will be automatically detected
            administrator.Phone = "0000";
            Assert.AreEqual(EntityState.Modified, context.Entry(administrator).State);

            // Saving the changes keeps entry tracked but sets its state to Unchanged
            Assert.AreEqual(1, context.SaveChanges());
            Assert.AreEqual(1, context.ChangeTracker.Entries().Count());
            Assert.AreEqual(EntityState.Unchanged, context.Entry(administrator).State);

            // Remove entry from change tracking is achieved by setting its state to detached
            context.Entry(administrator).State = EntityState.Detached;
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());

            // then, changes in the entry will not be detected - entry is still no longer in tracking
            administrator.Phone = "0001";
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());

            // The result of a query is always tracked after closing the query by casting the result to IEnumerable.
            // After requesting an IQueryable, the database command backing the query remains open!
            // Only a call that maps the result to an IEnumerable will close the command and put the resulting entries into tracking!
            var query1 = context.Administrators.Where(a => a.UserIdentityName != null);
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
            var entities1 = query1.ToList();
            Assert.AreEqual(1, context.ChangeTracker.Entries().Count());

            // Tracked results are in state Unchanged ...
            var entity1 = entities1.First();
            Assert.AreEqual(EntityState.Unchanged, context.Entry(entity1).State);

            // ... and modifications get detected
            entity1.Phone = "0002";
            Assert.AreEqual(EntityState.Modified, context.Entry(entity1).State);
            Assert.AreEqual(1, context.SaveChanges());
            Assert.AreEqual(EntityState.Unchanged, context.Entry(entity1).State);

            // Detach all entities
            context.DetachAllEntities();
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());

            // AsNoTracking()prevents tracking of the result of a query
            var entities2 = context.Administrators.Where(a => a.UserIdentityName != null).AsNoTracking().ToList();
            Assert.AreEqual(1, entities2.Count);
            Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
        }
    }
}