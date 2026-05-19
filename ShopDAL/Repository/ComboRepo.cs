using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class ComboRepo : IComboRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public ComboRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Combo> GetAllCombos()
        {
            return _dbContext.Combos.AsQueryable();
        }

        public Combo? GetById(int id)
        {
            return _dbContext.Combos
                .Include(c => c.ComboFoodItem)
                    .ThenInclude(cf => cf.FoodItem)
                .FirstOrDefault(c => c.ComboId == id);
        }
    }
}
