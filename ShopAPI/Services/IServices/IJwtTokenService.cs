using ShopDAL.Models;

namespace ShopAPI.Services.IServices
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
