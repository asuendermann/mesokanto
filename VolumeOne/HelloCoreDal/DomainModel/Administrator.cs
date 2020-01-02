using System.ComponentModel.DataAnnotations;

using HelloCoreCommons.Attributes;
using HelloCoreCommons.Models;

namespace HelloCoreDal.DomainModel {
    public abstract class Administrator : AbstractAuditableBase<int>, IAdministrator<int>, ITypePerHierarchy {
        [MaxLength(64)]
        public string Discriminator { get; set; }

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