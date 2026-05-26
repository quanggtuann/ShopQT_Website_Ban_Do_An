using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopDAL.Areas.Repository
{
    public class AdminCategoryRepo : IAdminCategoryRepo
    {
        private readonly ApplicationDbContext _context;
        public AdminCategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Category> GetAll()
        {
            return _context.Categorys.ToList();
        }
        public Category GetById(int id)
        {
            return _context.Categorys.FirstOrDefault(c => c.CategoryId == id);

        }
        public void Add(Category category)
        {
            _context.Categorys.Add(category);
            _context.SaveChanges();
        }
        public void Update(Category category)
        {
            var udct = _context.Categorys.FirstOrDefault(C => C.CategoryId == category.CategoryId);
            if (udct != null)
            {
                udct.Name = category.Name;
                udct.Description = category.Description;
                _context.SaveChanges();
            }
        }
        public void Deactive(int id)
        {
            var cate = _context.Categorys.FirstOrDefault(c => c.CategoryId == id);
            if (cate != null)
            {
               cate.IsActive = false;
                _context.SaveChanges();
            }
        }
        public void Activate(int id)
        {
            var cate = _context.Categorys.FirstOrDefault(c=>c.CategoryId==id);
            if(cate != null)
            {
                cate.IsActive = true;
                _context.SaveChanges();
            }
        }
    }
}
