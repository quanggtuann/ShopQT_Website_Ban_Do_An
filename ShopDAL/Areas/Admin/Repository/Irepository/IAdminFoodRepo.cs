using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminFoodRepo
    {
        List<FoodItem> GetAll();
        FoodItem GetById(int id);
        void Update(FoodItem item);
        void Add(FoodItem item);
        void Deactivate(int id);
        void AcTivate(int id);

    }
}
