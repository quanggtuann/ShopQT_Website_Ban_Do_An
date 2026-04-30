using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ShopDAL.Areas.Repository
{
    public class AdminComboRepo : IAdminComboRepo
    {
        private readonly ApplicationDbContext _context;
        public AdminComboRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Combo> Getall()
        {
            return _context.Combos.AsQueryable();
        }
        public Combo GetById(int id)
        {
            return _context.Combos
                .Include(c => c.ComboFoodItem)
                .ThenInclude(cf => cf.FoodItem)
                .FirstOrDefault(c => c.ComboId == id);
        }
        public void Add(Combo combo)
        {
            validate(combo);
            if (string.IsNullOrWhiteSpace(combo.Name))
            {
                throw new ArgumentException("combo name cannot be empty");
            }
            combo.IsAvailabale = true;
            _context.Combos.Add(combo);
            _context.SaveChanges();

        }
        public void Update(Combo combo)
        {
            validate(combo);            
            var udcb = _context.Combos
                .Include(c => c.ComboFoodItem)
                .FirstOrDefault(cb => cb.ComboId == combo.ComboId);
                
            if (udcb == null)
            {
                throw new KeyNotFoundException("combo not found");
            }           
            udcb.Name = combo.Name;
            udcb.Description = combo.Description;
            udcb.Price = combo.Price;
            udcb.ImagePath = combo.ImagePath;
            udcb.IsAvailabale = combo.IsAvailabale;
          
            if (udcb.ComboFoodItem != null && udcb.ComboFoodItem.Any())
            {
                _context.ComboFoodItems.RemoveRange(udcb.ComboFoodItem);
            }            
            if (combo.ComboFoodItem != null && combo.ComboFoodItem.Any())
            {
                foreach (var item in combo.ComboFoodItem)
                {
                    udcb.ComboFoodItem.Add(new ComboFoodItem
                    {
                        FoodItemID = item.FoodItemID,
                        Quantity = item.Quantity
                    });
                }
            }
            
            _context.SaveChanges();
        }
        private void validate(Combo combo)
        {
            var duplicateName = _context.Combos.Any(c => c.Name == combo.Name && c.ComboId != combo.ComboId);
            if (duplicateName)
            {
                throw new ArgumentException($"Combo name '{combo.Name}' already exists. Please choose a different name.");
            }
            if (string.IsNullOrWhiteSpace(combo.ImagePath))
            {
                throw new ArgumentException("An image is required for the combo item");
            }
            if (combo.ComboFoodItem == null || combo.ComboFoodItem.Count < 2)
            {
                throw new ArgumentException("Combo must include at least 2 food items");
            }
        }
        public void Deactivate(int id)
        {
            var combo = _context.Combos.FirstOrDefault(c => c.ComboId == id);
            if (combo == null)
            {
                throw new KeyNotFoundException("combo not found");
            }
            combo.IsAvailabale = false;
            _context.SaveChanges();
        }
        public void Activate(int id)
        {
            var combo = _context.Combos.FirstOrDefault(c => c.ComboId == id);
            if (combo == null)
            {
                throw new KeyNotFoundException("combo not found");
            }
            combo.IsAvailabale = true;
            _context.SaveChanges();
        }

    }
}
