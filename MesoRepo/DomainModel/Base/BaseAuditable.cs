using Commons.DomainModel;
using Commons.DomainModel.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModel.Base {
    public class BaseAuditable<TId> : IAuditable<TId> {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [MaxLength(DmConstants.MaxLength_256)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        public string ModifiedBy { get; set; }

    }
}