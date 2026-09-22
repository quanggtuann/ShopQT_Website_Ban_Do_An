namespace ShopDAL.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FoodId { get; set; }

        public DateTime CreateDate { get; set; }

        public User User { get; set; } = null!;

        public FoodItem FoodItem { get; set; } = null!;
    }
}
