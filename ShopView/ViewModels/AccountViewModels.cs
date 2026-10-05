using System.ComponentModel.DataAnnotations;

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

    public class CustomerProfileViewModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateorBirth { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class UpdateProfile
    {
        public string Email { get;set; }
        public string PhoneNumber { get; set; }
        public DateTime DateorBirth { get; set; }
    }
}
