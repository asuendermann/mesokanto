using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel;
using Commons.DomainModel.Domain;
using DomainModel.Base;

namespace DomainModel.Administration {
    public class Project : BaseAuditable<int>, IProject {
        public IEnumerable<ProjectAdministrator> ProjectAdministrators { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        [Required]
        public string Name { get; set; }

        [MaxLength] public string Description { get; set; }
    }
}