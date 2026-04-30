using ShopDAL.Context;

namespace ShopDAL.Repository
{
    public class AccountRepo
    {
        private readonly ApplicationDbContext _context;
        public AccountRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void CreateCartForUser(int userId)
        {

        }
    }
}
