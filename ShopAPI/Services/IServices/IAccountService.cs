using ShopDAL.Models;
using ShopAPI.DTOs;

namespace ShopAPI.Services.IServices
{
    public interface IAccountService
    {
        PagedResult<User> GetAllPaged(AccountFilterViewModel filter);
        User GetAccount(int id);
        User CreateAccount(User user);
        User UpdateAccount(int id, User user);
        void DeactivateAccount(int id);
        void ActivateAccount(int id);
    }
}
