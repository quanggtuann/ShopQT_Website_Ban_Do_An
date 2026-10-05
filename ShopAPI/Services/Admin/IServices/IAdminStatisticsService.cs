using ShopAPI.DTOs;

namespace ShopAPI.Services.IServices
{
    public interface IAdminStatisticsService
    {
        AdminStatisticsDto GetStatistics(DateTime? fromDate, DateTime? toDate);
    }
}
