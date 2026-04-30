using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopDAL.Areas.Repository
{
    public class AdminFoodRepo : IAdminFoodRepo
    {
        private readonly ApplicationDbContext _context;
        public AdminFoodRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<FoodItem> GetAll()
        {
            return _context.FoodItems
                .Include(f=>f.Category)
                .ToList();
        }
        public FoodItem GetById(int id)
        {
            return _context.FoodItems.FirstOrDefault(f => f.FoodItemId == id);
        }
        public void Add(FoodItem food)
        {
            Validate(food);
            _context.FoodItems.Add(food);
            _context.SaveChanges();
        }

        public void Update(FoodItem food)
        {
            Validate(food);
            var udfood = _context.FoodItems.FirstOrDefault(f => f.FoodItemId == food.FoodItemId);
            if (udfood != null)
            {
                udfood.Name = food.Name;
                udfood.Description = food.Description;
                udfood.Price = food.Price;
                udfood.CategoryId = food.CategoryId;
                udfood.ImagePath = food.ImagePath;
                udfood.IsAvailable = food.IsAvailable;
                _context.SaveChanges();
            }
        }
        private void Validate(FoodItem food)
        {
            if (string.IsNullOrEmpty(food.Name))
            {
                throw new ArgumentException("food name is required");
            }
            if (food.Price <= 0)
            {
                throw new ArgumentException("price must be greater than zero");
            }
            if (food.CategoryId <= 0)
            {
                throw new ArgumentException("A valid category must be selected");
            }
            var DuplicateName = _context.FoodItems
                  .Any(f => f.Name == food.Name && f.FoodItemId != food.FoodItemId);
            if (DuplicateName)
            {
                throw new ArgumentException("Food Name already existts. Please choose different name ");

            }
            if (food.ImagePath == null)
            {
                throw new ArgumentException("An Image is required for the food item");
            }
        }
        public void Deactivate(int id)
        {
            var food = _context.FoodItems.FirstOrDefault(f => f.FoodItemId == id);
            if (food != null)
            {
                food.IsAvailable = false;
                _context.SaveChanges();
            }
        }
        public void AcTivate(int id)
        {
            var food = _context.FoodItems.FirstOrDefault(f=>f.FoodItemId==id);
            if(food != null)
            {
                food.IsAvailable = true;
                _context.SaveChanges();
            }
        }
    }
}
