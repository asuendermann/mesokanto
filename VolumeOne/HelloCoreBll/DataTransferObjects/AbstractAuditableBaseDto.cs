using System;
using System.ComponentModel.DataAnnotations;

using HelloCoreCommons.DomainModel;

namespace HelloCoreBll.DataTransferObjects {
    public abstract class AbstractAuditableBaseDto<TId> : AbstractEntityBaseDto<TId>, IAuditableBase<TId> {
        public DateTime CreatedAt { get; set; }

        [MaxLength(256)]
        [Required]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [MaxLength(256)]
        public string ModifiedBy { get; set; }
    }
}