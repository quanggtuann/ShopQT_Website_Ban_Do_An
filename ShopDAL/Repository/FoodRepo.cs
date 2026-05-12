using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ShopDAL.Repository
{
    public class FoodRepo : IFoodRepo
    {
        private readonly ApplicationDbContext _context;
        public FoodRepo(ApplicationDbContext dbcontext)
        {
            _context = dbcontext;
        }
        public List<FoodItem> Getall()
        {
            return _context.FoodItems
                .Include(f => f.Category)
                .ToList();
        }
        public FoodItem GetById(int id)
        {
            return _context.FoodItems
                .Include(f => f.Category)
                .FirstOrDefault(fi=>fi.FoodItemId==id);
        }
    }

}
