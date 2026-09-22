using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IFavoriteRepo
    {
        List<Favorite> GetByUserId(int userId);
        Favorite GetByUserAndFood(int userId, int foodId);
        bool FoodExists(int foodId);
        void Add(Favorite favorite);
        void Remove(Favorite favorite);
        void Save();
    }
}
