using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using ShopAPI.DTOs;
namespace ShopAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAdminAccountRepo _accountRepo;
        public AccountService(IAdminAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public PagedResult<User> GetAllPaged(AccountFilterViewModel filter)
        {
            var accounts = _accountRepo.GetFiltered(
                filter.Keyword, 
                filter.IsActive, 
                filter.Role, 
                filter.SortBy, 
                filter.SortOrder);

            var totalItems = accounts.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)filter.pageSize);

            var pagedAccounts = accounts
                .Skip((filter.page - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .ToList();

            return new PagedResult<User>
            {
                Data = pagedAccounts,
                TotalItems = totalItems,
                CurrentPage = filter.page,
                TotalPages = totalPages
            };
        }
        public User GetAccount(int id)
        {
            var account = _accountRepo.GetById(id);
            if (account == null)
                throw new Exception("Account not found");
            return account;
        }
        public User CreateAccount(User user)
        {
            _accountRepo.Add(user);
            return user;
        }

        public User UpdateAccount(int id, User user)
        {
            if (id != user.UserID)
                throw new Exception("ID mismatch");
            
            _accountRepo.Update(user);
            return user;
        }

        public void DeactivateAccount(int id)
        {
            var account = _accountRepo.GetById(id);
            if (account == null)
                throw new Exception("Account not found");           
            _accountRepo.Deactive(id);
        }
        public void ActivateAccount(int id)
        {
            var account = _accountRepo.GetById(id);
            if (account == null)
                throw new Exception("Account not found");           
            _accountRepo.ActiveUser(id);
        }
    }
}
