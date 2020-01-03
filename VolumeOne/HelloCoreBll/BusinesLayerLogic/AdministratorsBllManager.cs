using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AutoMapper.Extensions.ExpressionMapping;

using HelloCoreBll.DataTransferObjects;

using HelloCoreDal.DomainModel;
using HelloCoreDal.Repository;

namespace HelloCoreBll.BusinesLayerLogic {
    public class AdministratorsBllManager : AbstractBllManager, IAdministratorsBllManager {
        public AdministratorsBllManager(IServiceProvider provider) : base(provider) {
            AdminRepo = (IGenericRepository<Administrator, int>)
                provider.GetService(typeof(IGenericRepository<Administrator, int>));
            MasterAdminRepo = (IGenericRepository<MasterAdministrator, int>)
                provider.GetService(typeof(IGenericRepository<MasterAdministrator, int>));
            ProjectAdminRepo = (IGenericRepository<ProjectAdministrator, int>)
                provider.GetService(typeof(IGenericRepository<ProjectAdministrator, int>));
        }

        public IGenericRepository<Administrator, int> AdminRepo { get; }

        public IGenericRepository<MasterAdministrator, int> MasterAdminRepo { get; }

        public IGenericRepository<ProjectAdministrator, int> ProjectAdminRepo { get; }

        public bool IsAdmin(string userIdentityName) {
            return AdminRepo.Read(m => m.UserIdentityName == userIdentityName).Any();
        }

        public bool IsMaster(string userIdentityName) {
            return MasterAdminRepo.Read(m => m.UserIdentityName == userIdentityName).Any();
        }

        public int CountAdministrators() {
            var count = AdminRepo.Count();
            return count;
        }

        public int CountMasterAdministrators() {
            var count = MasterAdminRepo.Count();
            return count;
        }

        public int CountProjectAdministrators() {
            var count = ProjectAdminRepo.Count();
            return count;
        }

        public IEnumerable<MasterAdministratorDto> ReadMasterAdministrators
            (params Expression<Func<MasterAdministratorDto, object>>[] paths) {
            var mappedPaths = new List<Expression<Func<MasterAdministrator, object>>>();
            if (null != paths) {
                foreach (var path in paths) {
                    var mappedPath = Mapper.MapExpression<Expression<Func<MasterAdministrator, object>>>(path);
                    mappedPaths.Add(mappedPath);
                }
            }

            return Mapper.Map<IEnumerable<MasterAdministratorDto>>(
                MasterAdminRepo.Read(mappedPaths.ToArray()));
        }
    }
}