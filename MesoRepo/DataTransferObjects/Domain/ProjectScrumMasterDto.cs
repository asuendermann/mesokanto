using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class ProjectScrumMasterDto : BaseAuditableDto<int>, IProjectScrumMaster {
        public ProjectDto Project { get; set; }

        public ScrumMasterDto ScrumMaster { get; set; }

        public int ProjectId { get; set; }

        public int ScrumMasterId { get; set; }
    }
}