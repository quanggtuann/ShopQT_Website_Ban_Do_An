using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IAddressRepo
    {
        Address? GetById(int id);
        IQueryable<Address> GetByUserId(int userId);
        void Add(Address address);
        void Update(Address address);
        void Delete(int id);
        void SetDefault(int UserId, int AddressId);
        Address? GetAddressDefault(int userId);
        void Save();
    }
}
