namespace ShopDAL.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string DetailAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}