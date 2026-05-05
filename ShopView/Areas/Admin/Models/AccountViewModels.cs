using System.ComponentModel.DataAnnotations;

namespace ShopView.Areas.Admin.Models
{
    // For Index view - Filter and list
    public class AccountIndexViewModel
    {
        public string KeyWord { get; set; }
        public bool? IsActive { get; set; }
        public string Role { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public List<UserListItem> Accounts { get; set; } = new();
    }

    // For displaying user in list
    public class UserListItem
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    // For Create/Edit user
    public class UserEditViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string Role { get; set; } = "customer";

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateorBirth { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
