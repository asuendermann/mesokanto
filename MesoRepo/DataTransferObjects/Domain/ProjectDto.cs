using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class ProjectDto : BaseAuditableDto<int>, IProject {
        public IEnumerable<ProjectTeamMemberDto> ProjectTeamMembers { get; set; }

        public IEnumerable<ProjectScrumMasterDto> ProjectScrumMasters { get; set; }

        public IEnumerable<ProjectOwnerDto> ProjectOwners { get; set; }

        public IEnumerable<BacklogItemDto> BacklogItems { get; set; }

        public string Identifier { get; set; }

        public string Title { get; set; }

        [MaxLength]
        public string Description { get; set; }
    }
}