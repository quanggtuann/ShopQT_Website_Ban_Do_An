using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IAccountRepo
    {
        void CreateCartForUser(int userId);
        User Getnameuser(string username);
        bool Login(string username, string password);
        bool Register(User registerUser);
        void Update(User updateuser);
    }
}
