using System.Collections.Generic;

namespace DomainModel.Scrum {
    public class Owner : TeamMember {
        public IEnumerable<ProjectOwner> OwnerProjects { get; set; }
    }
}