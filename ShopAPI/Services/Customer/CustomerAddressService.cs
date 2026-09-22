using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;
using System.Runtime.InteropServices;

namespace ShopAPI.Services.Customer
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly IAddressRepo _addressRepo;
        public CustomerAddressService(IAddressRepo addressRepo)
        {
            _addressRepo = addressRepo;
        }
        public Address? GetById(int id)
        {
            return _addressRepo.GetById(id);
        }
        public IQueryable<Address> GetByUserId(int userId)
        {
            return _addressRepo.GetByUserId(userId);
        }
        public Address? GetAddressDefault(int userId)
        {
            return _addressRepo.GetAddressDefault(userId);
        }
        public void Add(int userId, CreateAddressDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            if (string.IsNullOrWhiteSpace(dto.ReceiverName))
            {
                throw new Exception("Receiver name is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                throw new Exception("PhoneNumber name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.Province))
            {
                throw new Exception("Province name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.Ward))
            {
                throw new Exception("Ward name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.DetailAddress))
            {
                throw new Exception("Detail name is required");
            }
            var address = new Address
            {
                UserId = userId,
                ReceiverName = dto.ReceiverName,
                PhoneNumber = dto.PhoneNumber,
                Province = dto.Province,
                District = dto.District ?? string.Empty,
                Ward = dto.Ward,
                DetailAddress = dto.DetailAddress,
                IsDefault = false,
            };
            var hasAddress = _addressRepo.GetByUserId(userId).Any();

            if (!hasAddress)
            {
                address.IsDefault = true;
            }
            _addressRepo.Add(address);
            _addressRepo.Save();
        }
        public void Update(int userId, UpdateAddressDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var address = _addressRepo.GetById(dto.AddressId);

            if (address == null || address.UserId != userId)
            {
                throw new Exception("Address not found.");

            }
            if (string.IsNullOrWhiteSpace(dto.ReceiverName))
            {
                throw new Exception("Receiver name is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                throw new Exception("PhoneNumber name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.Province))
            {
                throw new Exception("Province name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.Ward))
            {
                throw new Exception("Ward name is required");
            }
            if (string.IsNullOrWhiteSpace(dto.DetailAddress))
            {
                throw new Exception("Detail name is required");
            }
            address.ReceiverName = dto.ReceiverName;
            address.PhoneNumber = dto.PhoneNumber;
            address.Province = dto.Province;
            address.District = dto.District ?? string.Empty;
            address.Ward = dto.Ward;
            address.DetailAddress = dto.DetailAddress;
            _addressRepo.Update(address);
            _addressRepo.Save();
        }
        public void Delete(int id)
        {
            var address = _addressRepo.GetById(id);
            if (address == null)
            {
                throw new KeyNotFoundException("Address not found.");
            }
            _addressRepo.Delete(id);
            _addressRepo.Save();
        }
        public void SetDefault(int userId, int addressId)
        {
            var address = _addressRepo.GetById(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException("Address not found.");
            }
            if (address.UserId != userId)
            {
                throw new KeyNotFoundException("This address does not belong to this user.");
            }
            _addressRepo.SetDefault(userId, addressId); 
            _addressRepo.Save();
        }
    }
}
