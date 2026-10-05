using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopDAL.Areas.Repository
{
    public class AdminDiscountCampaignRepo : IAdminDiscountCampaignRepo
    {
        private readonly ApplicationDbContext _context;

        public AdminDiscountCampaignRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<DiscountCampaign> GetAll()
        {
            return _context.DiscountCampaigns
                .Include(c => c.CampaignProducts)
                .ThenInclude(cp => cp.FoodItem)
                .Include(c => c.CampaignCombos)
                .ThenInclude(cc => cc.Combo)
                .AsNoTracking();
        }

        public DiscountCampaign? GetById(int id)
        {
            return _context.DiscountCampaigns
                .Include(c => c.CampaignProducts)
                .ThenInclude(cp => cp.FoodItem)
                .Include(c => c.CampaignCombos)
                .ThenInclude(cc => cc.Combo)
                .FirstOrDefault(c => c.DiscountCampaignId == id);
        }

        public bool ExistsByName(string campaignName, int? excludeId = null)
        {
            return _context.DiscountCampaigns.Any(c =>
                c.CampaignName.ToLower() == campaignName.ToLower()
                && (!excludeId.HasValue || c.DiscountCampaignId != excludeId.Value));
        }

        public void Add(DiscountCampaign campaign)
        {
            _context.DiscountCampaigns.Add(campaign);
            _context.SaveChanges();
        }

        public void Update(DiscountCampaign campaign)
        {
            var current = _context.DiscountCampaigns
                .Include(c => c.CampaignProducts)
                .Include(c => c.CampaignCombos)
                .FirstOrDefault(c => c.DiscountCampaignId == campaign.DiscountCampaignId);

            if (current == null)
            {
                throw new KeyNotFoundException("Discount campaign not found");
            }

            current.CampaignName = campaign.CampaignName;
            current.Description = campaign.Description;
            current.DiscountType = campaign.DiscountType;
            current.DiscountValue = campaign.DiscountValue;
            current.StartDate = campaign.StartDate;
            current.EndDate = campaign.EndDate;
            current.IsActive = campaign.IsActive;
            current.UpdatedAt = DateTime.Now;

            _context.DiscountCampaignProducts.RemoveRange(current.CampaignProducts);
            _context.DiscountCampaignCombos.RemoveRange(current.CampaignCombos);

            foreach (var product in campaign.CampaignProducts)
            {
                current.CampaignProducts.Add(new DiscountCampaignProduct
                {
                    FoodItemId = product.FoodItemId,
                    CreatedAt = DateTime.Now
                });
            }

            foreach (var combo in campaign.CampaignCombos)
            {
                current.CampaignCombos.Add(new DiscountCampaignCombo
                {
                    ComboId = combo.ComboId,
                    CreatedAt = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        public void Activate(int id)
        {
            SetActive(id, true);
        }

        public void Deactivate(int id)
        {
            SetActive(id, false);
        }

        private void SetActive(int id, bool isActive)
        {
            var campaign = _context.DiscountCampaigns.FirstOrDefault(c => c.DiscountCampaignId == id);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Discount campaign not found");
            }

            campaign.IsActive = isActive;
            campaign.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
    }
}
