using System.ComponentModel.DataAnnotations;
using ShopAPI.Validation;
using ShopView.Validation;

namespace ShopView.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.PasswordPattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Password))]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.EmailPattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Email))]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.PhonePattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Phone))]
        public string PhoneNumber { get; set; } = string.Empty;
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
        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.PasswordPattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Password))]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.PasswordConfirmation))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class UpdateProfile
    {
        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.EmailPattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Email))]
        public string Email { get;set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Required))]
        [RegularExpression(CustomerInputRules.PhonePattern, ErrorMessageResourceType = typeof(CustomerValidationMessages), ErrorMessageResourceName = nameof(CustomerValidationMessages.Phone))]
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateorBirth { get; set; }
    }
}
