using System.Collections.Generic;
using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class TeamMemberDto : BaseAuditableDto<int>, ITeamMember {
        public IEnumerable<ProjectTeamMemberDto> TeamMemberProjects { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}