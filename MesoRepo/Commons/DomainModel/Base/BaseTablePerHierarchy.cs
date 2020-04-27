using System.ComponentModel.DataAnnotations;

namespace Commons.DomainModel.Base {
    public class BaseTablePerHierarchy<TId> : BaseAuditable<TId> {
        [Required]
        [MaxLength(DmConstants.MaxLength_256)]
        public string Discriminator { get; set; }
    }
}