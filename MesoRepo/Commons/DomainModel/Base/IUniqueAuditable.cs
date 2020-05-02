namespace Commons.DomainModel.Base {
    public interface IUniqueAuditable {
        bool HasSameUniqueKey(object target);

    }
}
