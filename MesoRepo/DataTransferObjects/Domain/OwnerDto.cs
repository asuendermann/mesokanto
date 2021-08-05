using System.Collections.Generic;

namespace DataTransferObjects.Domain {
    public class OwnerDto : TeamMemberDto {
        public IEnumerable<ProjectOwnerDto> OwnerProjects { get; set; }
    }
}