using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eleven41.AspNetCore.MultiTenantIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantSample.Mvc.Models;

namespace MultiTenantSample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        UserManager<MultiTenantUser> _userManager;
        TenantManager<MultiTenantUser, IdentityTenant> _tenantManager;

        public HomeController(
            UserManager<MultiTenantUser> userManager,
            TenantManager<MultiTenantUser, IdentityTenant> tenantManager
            )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tenantManager = tenantManager ?? throw new ArgumentNullException(nameof(tenantManager));
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(this.User);
                var tenant = await _tenantManager.FindByIdAsync(user.TenantId.ToString());
                ViewData["TenantName"] = await _tenantManager.GetNameAsync(tenant);

                var users = await _tenantManager.GetTenantUsersAsync(tenant);
                ViewData["NumUsers"] = users.Count();
            }
            else
            {
                ViewData["TenantName"] = "Please sign-in";
                ViewData["NumUsers"] = null;
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
