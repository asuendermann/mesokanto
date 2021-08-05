using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Domain {
    public class ProjectTeamMember : BaseAuditable<int>, IProjectTeamMember {
        public Project Project { get; set; }

        public TeamMember TeamMember { get; set; }

        public int ProjectId { get; set; }

        public int TeamMemberId { get; set; }
    }
}