namespace ShopDAL.Models
{
    public class DiscountCampaignCombo
    {
        public int DiscountCampaignComboId { get; set; }

        public int DiscountCampaignId { get; set; }

        public int ComboId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DiscountCampaign DiscountCampaign { get; set; } = null!;

        public Combo Combo { get; set; } = null!;
    }
}