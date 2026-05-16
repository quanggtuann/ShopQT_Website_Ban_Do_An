namespace ShopView.ViewModels
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateofBirth { get; set; }
    }

    public class LoginResponse
    {
        public int userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
    }
}
