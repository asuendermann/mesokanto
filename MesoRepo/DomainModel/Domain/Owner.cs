using System.Collections.Generic;

namespace DomainModel.Domain {
    public class Owner : TeamMember {
        public IEnumerable<ProjectOwner> OwnerProjects { get; set; }
    }
}