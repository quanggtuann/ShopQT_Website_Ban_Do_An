using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class AddressRepo : IAddressRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public AddressRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Address? GetById(int id)
        {
            return _dbContext.Addresses.AsNoTracking().FirstOrDefault(ad => ad.AddressId == id);
        }
        public IQueryable<Address> GetByUserId(int userId)
        {
            return _dbContext.Addresses.AsNoTracking().Where(a=>a.UserId==userId);
        }
        public void Add(Address address)
        {
            _dbContext.Addresses.Add(address);
        }
        public void Update(Address address)
        {
            var adr=_dbContext.Addresses.FirstOrDefault(a=>a.AddressId==address.AddressId);
            if (adr == null) {
                throw new Exception("Address not found");
            }
            adr.ReceiverName = address.ReceiverName;
            adr.PhoneNumber = address.PhoneNumber;
            adr.Province = address.Province;
            adr.District = address.District;
            adr.Ward = address.Ward;
            adr.DetailAddress = address.DetailAddress;
        }
        public void Delete(int id)
        {
            var address = _dbContext.Addresses.FirstOrDefault(a => a.AddressId == id);
            if (address == null)
            {
                throw new KeyNotFoundException("Address not found");
            }
            _dbContext.Addresses.Remove(address);

        }
        public void SetDefault(int UserId, int AddressId)
        {
          var addresses=  _dbContext.Addresses.Where(a=>a.UserId == UserId).ToList();
            foreach(var item in addresses)
            {
                item.IsDefault=false;
            }
            var address=addresses.FirstOrDefault(a=>a.AddressId==AddressId);
            if(address == null)
            {
                throw new KeyNotFoundException("Address not found.");
            }
            address.IsDefault=true;
        }
        public Address? GetAddressDefault(int userId)
        {
          return  _dbContext.Addresses.AsNoTracking().FirstOrDefault(a => a.UserId == userId && a.IsDefault);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
