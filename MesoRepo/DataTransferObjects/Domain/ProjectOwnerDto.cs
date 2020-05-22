using System.Collections.Generic;
using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class ProjectOwnerDto : BaseAuditableDto<int>, IProjectOwner {
        public IEnumerable<BacklogItemDto> BacklogItems { get; set; }

        public ProjectDto Project { get; set; }

        public OwnerDto Owner { get; set; }

        public int ProjectId { get; set; }

        public int OwnerId { get; set; }
    }
}