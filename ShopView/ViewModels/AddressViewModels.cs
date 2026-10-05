using System.ComponentModel.DataAnnotations;

namespace ShopView.ViewModels
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string DetailAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class AddressFormViewModel
    {
        public int AddressId { get; set; }

        [Required]
        public string ReceiverName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Province { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        [Required]
        public string Ward { get; set; } = string.Empty;

        [Required]
        public string DetailAddress { get; set; } = string.Empty;
    }
    public class AddressPageViewModel
    {
        public List<AddressViewModel> Addresses { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
