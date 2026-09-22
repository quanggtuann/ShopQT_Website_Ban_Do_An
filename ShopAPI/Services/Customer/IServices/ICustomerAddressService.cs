using ShopAPI.DTOs;
using ShopDAL.Models;
using System.Security.Cryptography;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerAddressService
    {
        Address? GetById(int id);
        IQueryable<Address> GetByUserId(int userId);
        Address? GetAddressDefault(int userId);
        void Add(int userId, CreateAddressDto dto);
        void Update(int userId, UpdateAddressDto dto);
        void Delete(int id);
        void SetDefault(int userId, int addressId);
    }
}
