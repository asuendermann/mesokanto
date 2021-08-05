using System.ComponentModel.DataAnnotations;
using Commons.DomainModel;
using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Domain {
    public class BacklogItem : BaseAuditable<int>, IBacklogItem {
        public int ProjectOwnerId { get; set; }

        public ProjectOwner Author { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        [Required]
        public string Header { get; set; }

        [MaxLength]
        public string Content { get; set; }
    }
}