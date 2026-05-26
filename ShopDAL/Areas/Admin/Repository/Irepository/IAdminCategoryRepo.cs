using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminCategoryRepo
    {
        List<Category> GetAll();
        Category GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Deactive(int id);
        void Activate(int id);
    }
}
