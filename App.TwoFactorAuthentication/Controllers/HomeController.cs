using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using App.TwoFactorAuthentication.Models;

namespace App.TwoFactorAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Entry for the 2FA workflow
        [HttpGet]
        public IActionResult Index(TwoFactorAuthRequest request)
        {
            //TEST Flows

            //1. null request with return url
            //request.CallbackUrl = "Your Website URL";

            ////2. TwoFactorEnabled = true user request
            //request = new TwoFactorAuthRequest()
            //{
            //    CallbackUrl = "Your Website URL",
            //    IsTwoFactorEnabled = true,
            //    UserEmail = "your-email@email.com"
            //};
            //CurrentUser.SignedInUser = new CurrentUser { Email = request.UserEmail, TwoFactorEnabled = request.IsTwoFactorEnabled };

            //3.TwoFactorEnabled = false user request
            request = new TwoFactorAuthRequest()
            {
                CallbackUrl = "Your Website URL",
                IsTwoFactorEnabled = false,
                UserEmail = "your-email@email.com"
            };
            CurrentUser.SignedInUser = new CurrentUser { Email = request.UserEmail, TwoFactorEnabled = request.IsTwoFactorEnabled };

            return View(request);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
