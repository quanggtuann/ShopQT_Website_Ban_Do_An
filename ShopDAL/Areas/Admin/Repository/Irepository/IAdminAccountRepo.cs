using ShopDAL.Models;
namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminAccountRepo
    {
        List<User> GetAll();
        List<User> GetFiltered(string keyword, bool? isActive, string role, string sortBy, string sortOrder);
        User GetById(int id);
        void Update(User user);
        void Add(User user);
        void Deactive(int id);
        void ActiveUser(int id);
    }
}

