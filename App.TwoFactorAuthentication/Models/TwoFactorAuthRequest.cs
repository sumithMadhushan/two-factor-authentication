namespace App.TwoFactorAuthentication.Models
{
    public class TwoFactorAuthRequest
    {
        public string UserEmail { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string CallbackUrl { get; set; }
    }
}
