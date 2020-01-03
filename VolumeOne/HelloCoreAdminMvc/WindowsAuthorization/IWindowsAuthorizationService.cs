namespace HelloCoreAdminMvc.WindowsAuthorization {

    /// <summary>
    /// this service implementation is based upon https://damienbod.com/2018/04/19/asp-net-core-authorization-for-windows-local-accounts/.
    /// </summary>
    public interface IWindowsAuthorizationService {
        bool IsAdmin(string userIdentityName);

        bool IsMaster(string userIdentityName);
    }
}