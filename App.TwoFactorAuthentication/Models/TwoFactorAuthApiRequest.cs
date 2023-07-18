namespace App.TwoFactorAuthentication.Models
{

    // Request models for API endpoints
    public class TwoFactorAuthApiRequest
    {
        public string UserEmail { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
    }

    public class TwoFactorAuthVerifyRequest
    {
        public string Code { get; set; }
        public CurrentUser CurrentUser { get; set; }
    }

    public class VerifyCodeResponse
    {
        public bool IsVerifiedSuccess { get; set; }
        public string Message { get; set; }
    }

}
