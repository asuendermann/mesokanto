using System.Collections.Generic;

namespace DomainModel.Scrum {
    public class ScrumMaster : TeamMember {
        public IEnumerable<ProjectScrumMaster> ScrumMasterProjects { get; set; }
    }
}