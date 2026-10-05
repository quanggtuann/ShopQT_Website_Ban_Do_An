using Microsoft.EntityFrameworkCore;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer
{
    public class CustomerDiscountPriceService : ICustomerDiscountPriceService
    {
        private readonly ApplicationDbContext _context;

        public CustomerDiscountPriceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dictionary<int, DiscountPriceResult> GetFoodDiscounts(IEnumerable<int> foodItemIds)
        {
            var ids = foodItemIds.Distinct().ToList();
            var prices = _context.FoodItems
                .AsNoTracking()
                .Where(f => ids.Contains(f.FoodItemId))
                .Select(f => new { f.FoodItemId, f.Price })
                .ToDictionary(f => f.FoodItemId, f => f.Price);

            var now = DateTime.Now;
            var campaignProducts = _context.DiscountCampaignProducts
                .AsNoTracking()
                .Include(cp => cp.DiscountCampaign)
                .Where(cp => ids.Contains(cp.FoodItemId)
                    && cp.DiscountCampaign.IsActive
                    && cp.DiscountCampaign.StartDate <= now
                    && cp.DiscountCampaign.EndDate >= now)
                .ToList();

            return prices.ToDictionary(
                item => item.Key,
                item => GetBestDiscount(item.Value, campaignProducts
                    .Where(cp => cp.FoodItemId == item.Key)
                    .Select(cp => cp.DiscountCampaign)));
        }

        public Dictionary<int, DiscountPriceResult> GetComboDiscounts(IEnumerable<int> comboIds)
        {
            var ids = comboIds.Distinct().ToList();
            var prices = _context.Combos
                .AsNoTracking()
                .Where(c => ids.Contains(c.ComboId))
                .Select(c => new { c.ComboId, c.Price })
                .ToDictionary(c => c.ComboId, c => c.Price);

            var now = DateTime.Now;
            var campaignCombos = _context.DiscountCampaignCombos
                .AsNoTracking()
                .Include(cc => cc.DiscountCampaign)
                .Where(cc => ids.Contains(cc.ComboId)
                    && cc.DiscountCampaign.IsActive
                    && cc.DiscountCampaign.StartDate <= now
                    && cc.DiscountCampaign.EndDate >= now)
                .ToList();

            return prices.ToDictionary(
                item => item.Key,
                item => GetBestDiscount(item.Value, campaignCombos
                    .Where(cc => cc.ComboId == item.Key)
                    .Select(cc => cc.DiscountCampaign)));
        }

        public DiscountPriceResult GetFoodDiscount(int foodItemId, decimal originalPrice)
        {
            return GetFoodDiscounts(new[] { foodItemId }).GetValueOrDefault(foodItemId)
                ?? NoDiscount(originalPrice);
        }

        public DiscountPriceResult GetComboDiscount(int comboId, decimal originalPrice)
        {
            return GetComboDiscounts(new[] { comboId }).GetValueOrDefault(comboId)
                ?? NoDiscount(originalPrice);
        }

        private static DiscountPriceResult GetBestDiscount(decimal originalPrice, IEnumerable<DiscountCampaign> campaigns)
        {
            var best = campaigns
                .Select(campaign => new
                {
                    Campaign = campaign,
                    FinalPrice = CalculateFinalPrice(originalPrice, campaign)
                })
                .OrderBy(result => result.FinalPrice)
                .FirstOrDefault();

            if (best == null)
            {
                return NoDiscount(originalPrice);
            }

            return new DiscountPriceResult
            {
                OriginalPrice = originalPrice,
                FinalPrice = best.FinalPrice,
                CampaignName = best.Campaign.CampaignName,
                DiscountValue = best.Campaign.DiscountValue,
                DiscountType = best.Campaign.DiscountType.ToString()
            };
        }

        private static decimal CalculateFinalPrice(decimal originalPrice, DiscountCampaign campaign)
        {
            var finalPrice = campaign.DiscountType == DiscountType.Percent
                ? originalPrice - (originalPrice * campaign.DiscountValue / 100)
                : originalPrice - campaign.DiscountValue;

            return Math.Max(0, Math.Round(finalPrice, 0));
        }

        private static DiscountPriceResult NoDiscount(decimal originalPrice)
        {
            return new DiscountPriceResult
            {
                OriginalPrice = originalPrice,
                FinalPrice = originalPrice
            };
        }
    }
}
