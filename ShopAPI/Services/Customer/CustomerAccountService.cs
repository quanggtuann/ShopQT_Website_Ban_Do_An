using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;
using ShopAPI.Validation;

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
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (!CustomerInputRules.IsValidPassword(user.Password))
                throw new ArgumentException("Password must be at least 7 characters and include lowercase, uppercase, and a digit.");
            if (!CustomerInputRules.IsValidPhone(user.PhoneNumber))
                throw new ArgumentException("Phone number must contain exactly 10 digits and start with 0.");
            if (!CustomerInputRules.IsValidEmail(user.Email))
                throw new ArgumentException("Enter a valid email address ending in @gmail.com.");

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
            if (updateProfileDto == null)
                throw new ArgumentNullException(nameof(updateProfileDto));
            if (!CustomerInputRules.IsValidPhone(updateProfileDto.PhoneNumber))
                throw new ArgumentException("Phone number must contain exactly 10 digits and start with 0.");
            if (!CustomerInputRules.IsValidEmail(updateProfileDto.Email))
                throw new ArgumentException("Enter a valid email address ending in @gmail.com.");

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
            if (!CustomerInputRules.IsValidPassword(newPassword))
                throw new ArgumentException("Password must be at least 7 characters and include lowercase, uppercase, and a digit.");

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
