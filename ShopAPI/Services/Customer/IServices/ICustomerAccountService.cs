using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerAccountService
    {
        int Register(User user);

        LoginResponseDto Login(LoginRequest request);

        User GetProfile(int id);

        void UpdateProfile( int id,UpdateProfileDto updateProfileDto);

        void ChangePassword(int userId, string currentPassword, string newPassword);
    }
}
