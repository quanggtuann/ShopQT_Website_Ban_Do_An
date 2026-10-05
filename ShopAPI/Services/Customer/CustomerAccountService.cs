using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IJwtTokenService _jwtTokenService;

        public CustomerAccountService(
            IAccountRepo accountRepo,
            IJwtTokenService jwtTokenService)
        {
            _accountRepo = accountRepo;
            _jwtTokenService = jwtTokenService;
        }

        public int Register(User user)
        {
            user.Role = "customer";

            user.IsActive = true;

            _accountRepo.Register(user);

            return user.UserID;
        }

        public LoginResponseDto Login(LoginRequest request)
        {
            _accountRepo.Login(request.Username, request.Password);

            var user = _accountRepo.Getnameuser(request.Username);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var token = _jwtTokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Success = true,
                UserId = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public User GetProfile(int id)
        {
            var user = _accountRepo.GetById(id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public void UpdateProfile(int id,UpdateProfileDto updateProfileDto )
        {
            var user = _accountRepo.GetById(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.PhoneNumber = updateProfileDto.PhoneNumber;
            user.DateorBirth = updateProfileDto.DateorBirth;
            user.Email = updateProfileDto.Email;
            _accountRepo.Save();
        }

        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _accountRepo.GetById(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.Equals(user.Password, currentPassword, StringComparison.Ordinal))
            {
                throw new Exception("Current password is incorrect");
            }

            user.Password = newPassword;
            _accountRepo.Update(user);
        }
    }
}
