namespace ShopAPI.DTOs
{
    public class AddressDto
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

    public class CreateAddressDto
    {
        public string ReceiverName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public string DetailAddress { get; set; } = string.Empty;
    }

    public class UpdateAddressDto
    {
        public int AddressId { get; set; }

        public string ReceiverName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public string DetailAddress { get; set; } = string.Empty;
    }
    public class ProvinceDto
    {
        public string Province { get; set; } = string.Empty;

        public List<string> Wards { get; set; } = new();

        public List<DistrictDto> Districts { get; set; } = new();
    }

    public class DistrictDto
    {
        public string District { get; set; } = string.Empty;

        public List<string> Wards { get; set; } = new();
    }
}
