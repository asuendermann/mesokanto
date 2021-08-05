using System.Collections.Generic;

namespace DomainModel.Domain {
    public class ScrumMaster : TeamMember {
        public IEnumerable<ProjectScrumMaster> ScrumMasterProjects { get; set; }
    }
}