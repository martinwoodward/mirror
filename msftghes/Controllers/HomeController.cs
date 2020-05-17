using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using msftghes.Models;
using Microsoft.ApplicationInsights;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Http;

namespace msftghes.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private TelemetryClient telemetry;
        private readonly IWebHostEnvironment env;

        public HomeController(IWebHostEnvironment env, TelemetryClient telemetry)
        {
            this.telemetry = telemetry;
            this.env = env;
        }

        [Authorize(Roles = "Staff")] // Everyone
        public IActionResult Index()
        {
            ViewData["User"] = HttpContext.User;
            this.telemetry.TrackEvent("homepage-view", new Dictionary<string, string> { {"username", HttpContext.User.Identity.Name } });
            return View();
        }

        [Authorize(Roles = "Staff")] // Everyone
        public IActionResult CreateOrganization()
        {
            string CreateOrgURL = "https://github.com/account/organizations/new?plan=team_free";

            ViewData["User"] = HttpContext.User;
            this.telemetry.TrackEvent("create-org", new Dictionary<string, string> { { "username", HttpContext.User.Identity.Name } });
            return Redirect(CreateOrgURL); // Go to the GitHub Organization set up page. 
        }

        [Authorize(Roles = "Staff")] // Everyone
        public IActionResult RedeemCoupon()
        {
            string CouponURL = "https://woodwardweb.com"; 

            ViewData["User"] = HttpContext.User;
            this.telemetry.TrackEvent("redeem-coupon", new Dictionary<string, string> { { "username", HttpContext.User.Identity.Name } });
            return Redirect(CouponURL); // Go to the GitHub Coupon redemption page.
        }

        [Authorize(Roles = "Staff")] // Everyone
        public IActionResult GetKey()
        {
            // Replace this with the current valid key and place the key file in /Keys/keyid.ghl
            string keyid = "351c1d";

            this.telemetry.TrackEvent("download-key", 
                new Dictionary<string, string> { 
                    { "username", HttpContext.User.Identity.Name },
                    { "key-id", keyid } });

            var file = Path.Combine(env.ContentRootPath,"Keys", keyid + ".ghl");

            return PhysicalFile(file, "application/octet-stream", "ghes-" 
                + keyid + "-" 
                + HttpContext.User.Identity.Name.Replace('@', '-').Replace('.', '-') + ".ghl");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}