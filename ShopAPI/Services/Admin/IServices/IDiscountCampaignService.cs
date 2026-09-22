using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.IServices
{
    public interface IDiscountCampaignService
    {
        PagedResult<DiscountCampaignDto> GetAll(DiscountCampaignFilterViewModel filter);
        DiscountCampaignDto GetById(int id);
        DiscountCampaignDto Create(CreateDiscountCampaignRequest request);
        DiscountCampaignDto Update(int id, UpdateDiscountCampaignRequest request);
        void Activate(int id);
        void Deactivate(int id);
        bool CheckDuplicateName(string campaignName, int? excludeId = null);
    }
}
