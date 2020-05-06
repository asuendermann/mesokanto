using Commons.DomainModel.Domain;
using DomainModel.Base;

namespace DomainModel.Administration {
    public class ProjectAdministrator : BaseAuditable<int>, IProjectAdministrator {
        public Project Project { get; set; }

        public Administrator Administrator { get; set; }

        public int ProjectId { get; set; }

        public int AdministratorId { get; set; }
    }
}