using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class ProjectTeamMemberDto : BaseAuditableDto<int>, IProjectTeamMember {
        public ProjectDto Project { get; set; }

        public TeamMemberDto TeamMember { get; set; }

        public int ProjectId { get; set; }

        public int TeamMemberId { get; set; }
    }
}