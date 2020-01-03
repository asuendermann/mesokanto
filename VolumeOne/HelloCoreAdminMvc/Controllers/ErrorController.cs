using System.Diagnostics;

using HelloCoreAdminMvc.Models;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HelloCoreAdminMvc.Controllers {
    public class ErrorController : Controller {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index() {
            // Retrieve error information in case of internal errors
            var error = HttpContext
                .Features
                .Get<IExceptionHandlerFeature>();

            // Use the information about the exception 
            var exception = error?.Error;
            var model = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Exception = exception
            };
            return View("Error", model);
        }
    }
}