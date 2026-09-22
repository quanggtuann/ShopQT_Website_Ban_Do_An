namespace ShopDAL.Models
{
    public class DiscountCampaignProduct
    {
        public int DiscountCampaignProductId { get; set; }

        public int DiscountCampaignId { get; set; }

        public int FoodItemId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DiscountCampaign DiscountCampaign { get; set; } = null!;

        public FoodItem FoodItem { get; set; } = null!;
    }
}
