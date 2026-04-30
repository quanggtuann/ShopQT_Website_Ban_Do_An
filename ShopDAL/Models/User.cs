using System.ComponentModel.DataAnnotations;
namespace ShopDAL.Models
{
    public class User
    {
        public int UserID {  get; set; }
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password {  get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; } = "customer";
        [Required]
        public DateTime DateorBirth { get; set; }
        public bool IsActive {  get; set; }=true;
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual Cart Cart { get; set; }
    }
}

