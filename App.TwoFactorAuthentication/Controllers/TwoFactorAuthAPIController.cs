using App.TwoFactorAuthentication.Models;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using System;

namespace App.TwoFactorAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFactorAuthAPIController : ControllerBase
    {
        private const string TENANT = "TestTenant";
        private const string PAT = "#SECRETKEY";
        private const string TENANT_SECRETKEY = "myverysecretkey";

        [HttpPost]
        [Route("GenerateQRCode")]
        public IActionResult GenerateQRCode(TwoFactorAuthApiRequest request)
        {
            
            if (!IsValidRequest(Request.Headers["TENANT"], Request.Headers["PAT"]))
            {
                var invalidOrgRes = new VerifyCodeResponse()
                {
                    IsVerifiedSuccess = false,
                    Message = "Invalid Organization!"
                };

                return Ok(invalidOrgRes);
            }

            CurrentUser.SignedInUser = new CurrentUser { Email = request.UserEmail, TwoFactorEnabled = request.IsTwoFactorEnabled };

            var user = CurrentUser.SignedInUser;

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode(TENANT, user.Email, TwoFactorKey(user), false, 3);
            var setupCode = setupInfo.ManualEntryKey;
            var barcodeImageUrl = setupInfo.QrCodeSetupImageUrl;

            // Return the QR code URL and setup code
            return Ok(new { QRCodeUrl = barcodeImageUrl, SetupCode = setupCode });
        }


        [HttpPost]
        [Route("Verify2FACode")]
        public IActionResult VerifyCode(TwoFactorAuthVerifyRequest request)
        {
            if (!IsValidRequest(Request.Headers["TENANT"], Request.Headers["PAT"]))
            {
                var invalidOrgRes = new VerifyCodeResponse()
                {
                    IsVerifiedSuccess = false,
                    Message = "Invalid Organization!"
                };

                return Ok(invalidOrgRes);
            }

            CurrentUser.SignedInUser = request.CurrentUser;

            var user = CurrentUser.SignedInUser;
            if (user == null) { return BadRequest("User Details not found"); }
            if (!user.TwoFactorEnabled)
            {
                // Two-Factor Authentication is not enabled for the user
                return BadRequest("Two-Factor Authentication is not enabled");
            }

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var currentPin = twoFactor.GetCurrentPIN(TwoFactorKey(user));
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), request.Code);

            if (currentPin != request.Code && isValid)
            {
                var expiredRes = new VerifyCodeResponse()
                {
                    IsVerifiedSuccess = false,
                    Message = "Code is expired!"
                };

                return Ok(expiredRes);
            }

            var res = new VerifyCodeResponse()
            {
                IsVerifiedSuccess = isValid,
                Message = !isValid ? "Invalid code!" : "Access granted!"
            };

            return Ok(res);
        }


        private static string TwoFactorKey(CurrentUser user)
        {
            return $"{TENANT_SECRETKEY}+{user.Email}";
        }

        private bool IsValidRequest(string organization, string pat)
        {
            if (string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(pat)) { return false; }

            if (organization == TENANT && pat == PAT) { return true; }

            return false;
        }
    }
}