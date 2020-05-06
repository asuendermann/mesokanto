using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Scrum {
    public class ProjectScrumMaster : BaseAuditable<int>, IProjectScrumMaster {
        public Project Project { get; set; }

        public ScrumMaster ScrumMaster { get; set; }

        public int ProjectId { get; set; }

        public int ScrumMasterId { get; set; }
    }
}