using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using HelloCoreCommons.Models;

namespace HelloCoreDal.DomainModel {
    public abstract class AbstractEntityBase<T> : IEntityBase<T> {
        [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
    }
}