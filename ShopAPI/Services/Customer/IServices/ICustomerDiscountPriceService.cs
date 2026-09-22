namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerDiscountPriceService
    {
        Dictionary<int, DiscountPriceResult> GetFoodDiscounts(IEnumerable<int> foodItemIds);
        Dictionary<int, DiscountPriceResult> GetComboDiscounts(IEnumerable<int> comboIds);
        DiscountPriceResult GetFoodDiscount(int foodItemId, decimal originalPrice);
        DiscountPriceResult GetComboDiscount(int comboId, decimal originalPrice);
    }

    public class DiscountPriceResult
    {
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public bool HasDiscount => FinalPrice < OriginalPrice;
        public string? CampaignName { get; set; }
        public decimal? DiscountValue { get; set; }
        public string? DiscountType { get; set; }
    }
}
