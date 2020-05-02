using Commons.DomainModel;
using Commons.DomainModel.Attributes;
using Commons.DomainModel.Base;
using DomainModel.Base;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.Administration {
    public class Administrator : BaseTablePerHierarchy<int> {
        [MaxLength(DmConstants.MaxLength_32)]
        [Required]
        public string UserIdentityName { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        [Required]
        public string Name { get; set; }

        [MaxLength(DmConstants.MaxLength_1024)]
        [Required]
        [Email]
        public string Email { get; set; }

        [MaxLength(DmConstants.MaxLength_64)]
        [Required]
        public string Phone { get; set; }
    }
}