using System.Collections.Generic;
using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Domain {
    public class ProjectOwner : BaseAuditable<int>, IProjectOwner {
        public IEnumerable<BacklogItem> BacklogItems { get; set; }

        public Project Project { get; set; }

        public Owner Owner { get; set; }

        public int ProjectId { get; set; }

        public int OwnerId { get; set; }
    }
}