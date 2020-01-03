using System.ComponentModel.DataAnnotations;

using HelloCoreCommons.DomainModel;

namespace HelloCoreBll.DataTransferObjects {
    public abstract class AbstractTypePerHierarchyBaseDto<TId> : AbstractAuditableBaseDto<TId>, ITypePerHierarchy {
        [MaxLength(64)]
        [Required]
        public string Discriminator { get; set; }
    }
}