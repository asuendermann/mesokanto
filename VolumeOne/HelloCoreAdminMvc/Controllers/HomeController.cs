using System;

using HelloCoreAdminMvc.WindowsAuthorization;

using HelloCoreVm;
using HelloCoreVm.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloCoreAdminMvc.Controllers {
    public class HomeController : AbstractAdminController {

        public HomeController(IServiceProvider serviceProvider) {
        }

        public IActionResult Index([FromServices]IViewModelManager adminVmManager) {
            var model = adminVmManager?.GetHomeViewModel() ?? new HomeViewModel();
            return View(model);
        }

    }
}
