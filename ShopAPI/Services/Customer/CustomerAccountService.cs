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
            var user = _accountRepo.Getnameuser(id.ToString());

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public void UpdateProfile(User user)
        {
            _accountRepo.Update(user);
        }
    }
}