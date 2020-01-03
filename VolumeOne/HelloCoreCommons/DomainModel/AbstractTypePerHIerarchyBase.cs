using System.ComponentModel.DataAnnotations;

namespace HelloCoreCommons.DomainModel {
    public abstract class AbstractTypePerHierarchyBase<TId> : AbstractAuditableBase<TId> {
        [MaxLength(64)]
        [Required]
        public string Discriminator { get; set; }
    }
}