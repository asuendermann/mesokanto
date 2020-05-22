using Commons.DomainModel.Scrum;
using DataTransferObjects.Base;

namespace DataTransferObjects.Domain {
    public class BacklogItemDto : BaseAuditableDto<int>, IBacklogItem {
        public ProjectOwnerDto Author { get; set; }

        public ProjectDto Project { get; set; }

        public int ProjectOwnerId { get; set; }

        public int ProjectId { get; set; }

        public string Header { get; set; }

        public string Content { get; set; }
    }
}