using System.ComponentModel.DataAnnotations;

using HelloCoreCommons.Attributes;
using HelloCoreCommons.DomainModel;

namespace HelloCoreDal.DomainModel {
    public class Administrator : AbstractTypePerHierarchyBase<int>, IAdministrator<int>, ITypePerHierarchy {
        [MaxLength(32)]
        [Required]
        public string UserIdentityName { get; set; }

        [MaxLength(256)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        [Email]
        public string Email { get; set; }

        [MaxLength(32)]
        [Required]
        public string Phone { get; set; }
    }
}