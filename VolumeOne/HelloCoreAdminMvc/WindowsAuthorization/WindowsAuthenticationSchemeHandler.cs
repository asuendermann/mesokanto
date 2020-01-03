using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class WindowsAuthenticationSchemeHandler : IAuthenticationHandler {

        public const string SchemaName = "WindowsAuthenticationSchema";
        private HttpContext httpContext;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) {
            this.httpContext = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties) {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties) {
            httpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            httpContext.Response.Redirect("Error");
            return Task.CompletedTask;
        }
    }
}
