using System;
using System.ComponentModel.DataAnnotations;

namespace HelloCoreCommons.DomainModel {
    public abstract class AbstractAuditableBase<T> : AbstractEntityBase<T>, IAuditableBase<T> {
        public DateTime CreatedAt { get; set; }

        [MaxLength(256)] 
        [Required]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [MaxLength(256)] 
        public string ModifiedBy { get; set; }
    }
}