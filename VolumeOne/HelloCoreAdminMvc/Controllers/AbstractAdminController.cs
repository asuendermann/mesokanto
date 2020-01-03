using HelloCoreAdminMvc.WindowsAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelloCoreAdminMvc.Controllers {
    [Authorize(Policy = IsWindowsAdminRequirement.IsWindowsAdminRequirementPolicy)]
    public class AbstractAdminController : Controller {
        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            base.OnActionExecuted(context);
        }
    }
}