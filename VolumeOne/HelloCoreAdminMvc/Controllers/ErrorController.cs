using System.Diagnostics;

using HelloCoreAdminMvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace HelloCoreAdminMvc.Controllers {
    public class ErrorController : Controller {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index() {
            return View("Error", new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}