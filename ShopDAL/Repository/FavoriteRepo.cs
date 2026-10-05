using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class FavoriteRepo : IFavoriteRepo
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Favorite> GetByUserId(int userId)
        {
            return _context.Favorites
                .AsNoTracking()
                .Include(f => f.FoodItem)
                .ThenInclude(f => f.Category)
                .Where(f => f.UserId == userId && f.FoodItem.IsAvailable)
                .OrderByDescending(f => f.CreateDate)
                .ToList();
        }

        public Favorite GetByUserAndFood(int userId, int foodId)
        {
            return _context.Favorites
                .Include(f => f.FoodItem)
                .ThenInclude(f => f.Category)
                .FirstOrDefault(f => f.UserId == userId && f.FoodId == foodId);
        }

        public bool FoodExists(int foodId)
        {
            return _context.FoodItems.Any(f => f.FoodItemId == foodId && f.IsAvailable);
        }

        public void Add(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
        }

        public void Remove(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
