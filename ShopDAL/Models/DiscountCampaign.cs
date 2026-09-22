namespace ShopDAL.Models
{
    public enum DiscountType
    {
        Percent = 1,
        FixedAmount = 2
    }
    public class DiscountCampaign
    {
        public int DiscountCampaignId { get; set; }

        public string CampaignName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DiscountType DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<DiscountCampaignProduct> CampaignProducts { get; set; }
            = new List<DiscountCampaignProduct>();
        public ICollection<DiscountCampaignCombo> CampaignCombos { get; set; }
    = new List<DiscountCampaignCombo>();
    }
}
