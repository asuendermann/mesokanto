namespace HelloCoreCommons.Models {
    public interface IAdministrator<T> : IAuditableBase<T> {

        string UserIdentityName { get; set; }

        string Name { get; set; }

        string Email { get; set; }

        string Phone { get; set; }
    }
}