using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloCoreCommons.DomainModel {
    public abstract class AbstractEntityBase<T> : IEntityBase<T> {
        [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
    }
}