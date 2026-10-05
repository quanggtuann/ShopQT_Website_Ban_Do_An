using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminDiscountCampaignRepo
    {
        IQueryable<DiscountCampaign> GetAll();
        DiscountCampaign? GetById(int id);
        bool ExistsByName(string campaignName, int? excludeId = null);
        void Add(DiscountCampaign campaign);
        void Update(DiscountCampaign campaign);
        void Activate(int id);
        void Deactivate(int id);
    }
}
