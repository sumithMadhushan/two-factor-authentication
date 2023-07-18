using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using App.TwoFactorAuthentication.Models;

namespace App.TwoFactorAuthentication.Controllers
{
    public class TwoFactorAuthenticationController : Controller
    {
        [HttpGet]
        public ActionResult Enable(string callbackUrl)
        {
            var user = CurrentUser.SignedInUser;
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode("Test-Tenant", user.Email, TwoFactorKey(user), false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ViewData["CallbackUrl"] = callbackUrl;

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"].ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Enable(string inputCode, string callbackUrl)
        {
            var user = CurrentUser.SignedInUser;
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
            if (!isValid)
            {
                var encodedCallbackUrl = System.Uri.EscapeDataString(callbackUrl);
                var url = $"/twofactorauthentication/enable?callbackUrl={encodedCallbackUrl}";
                TempData["ErrorMessage"] = "Invalid input code. Enter Valid Code.!";

                return Redirect(url);
            }

            user.TwoFactorEnabled = true;
            var returnUrl = string.Format("{0}?2FAEnabled={1}", callbackUrl, false);
            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Disable(string callbackUrl)
        {
            ViewData["CallbackUrl"] = callbackUrl;
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"].ToString();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Disable(string inputCode, string callbackUrl)
        {
            var user = CurrentUser.SignedInUser;
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
            if (!isValid)
            {
                var encodedCallbackUrl = System.Uri.EscapeDataString(callbackUrl);
                var url = $"/twofactorauthentication/disable?callbackUrl={encodedCallbackUrl}";
                TempData["ErrorMessage"] = "Invalid input code. Enter Valid Code.!";
                return Redirect(url);
            }

            user.TwoFactorEnabled = false;
            var returnUrl = string.Format("{0}?2FAEnabled={1}", callbackUrl, true);
            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Authorize(string callbackUrl)
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"].ToString();
            }

            ViewData["CallbackUrl"] = callbackUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Authorize(string inputCode, string callbackUrl)
        {
            var user = CurrentUser.SignedInUser;
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
            if (!isValid)
            {
                var encodedCallbackUrl = System.Uri.EscapeDataString(callbackUrl);
                var url = $"/twofactorauthentication/authorize?callbackUrl={encodedCallbackUrl}";
                
                TempData["ErrorMessage"] = "Invalid input code. Enter Valid Code.!";

                return Redirect(url);
            }

            var returnUrl = string.Format("{0}?authorized={1}", callbackUrl, true);
            return Redirect(returnUrl);
        }

        private static string TwoFactorKey(CurrentUser user)
        {
            return $"myverysecretkey+{user.Email}";
        }
    }
}
