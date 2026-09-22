using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminStatisticsRepo
    {
        IQueryable<Order> GetOrders();
        IQueryable<User> GetUsers();
        IQueryable<FoodItem> GetFoods();
        IQueryable<Combo> GetCombos();
    }
}
