using System.Collections.Generic;

namespace DataTransferObjects.Domain {
    public class ScrumMasterDto : TeamMemberDto {
        public IEnumerable<ProjectScrumMasterDto> ScrumMasterProjects { get; set; }
    }
}