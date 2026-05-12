using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IFoodRepo 
    {
        List<FoodItem> Getall();
        FoodItem GetById(int id);
    }
}
