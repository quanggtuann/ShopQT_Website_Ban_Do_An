using System.ComponentModel.DataAnnotations;
using ShopView.Areas.Admin.ViewModel;

namespace ShopView.Areas.Admin.Models
{
    public class AccountFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
        public string? SortBy { get; set; } = "id";
        public string? SortOrder { get; set; } = "asc";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }

    public class AccountIndexViewModel
    {
        public AccountFilterViewModel Filter { get; set; } = new();
        public PagedResponse<UserListItem> PagedResult { get; set; } = new();
    }
    public class UserListItem
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
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
