using Commons.DomainModel;
using Commons.DomainModel.Base;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.Base {
    public class BaseTablePerHierarchy<TId> : BaseAuditable<TId>, ITablePerHierarchy<TId> {
        [MaxLength(DmConstants.MaxLength_256)]
        public string Discriminator { get; set; }
    }
}