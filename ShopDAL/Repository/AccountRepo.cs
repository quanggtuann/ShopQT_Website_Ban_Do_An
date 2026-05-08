using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class AccountRepo : IAccountRepo
    {
        private readonly ApplicationDbContext _context;
        public AccountRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void CreateCartForUser(int userId)
        {
            var cart = new Cart { UserID = userId }; 
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }
        public User Getnameuser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public bool Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("User is not exists");
            }
            if (!password.Equals(user.Password))
            {
                throw new Exception("Password is not true");
            }
            if (user.IsActive == null)
            {
                throw new Exception("Cannot login because this user is deactived.");
            }
            return true;
        }
        public bool Register(User registerUser)
        {
            if (_context.Users.Any(u => u.Username == registerUser.Username))
            {
                throw new Exception("User already exists");
            }
            if (_context.Users.Any(u => u.Email == registerUser.Email))
            {
                throw new Exception("Email already exists");
            }
            
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Users.Add(registerUser);
                _context.SaveChanges();
                
                // Tạo Cart trong cùng transaction
                var cart = new Cart { UserID = registerUser.UserID };
                _context.Carts.Add(cart);
                _context.SaveChanges();
                
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Update(User updateuser)
        {
            _context.Users.Update(updateuser);
            _context.SaveChanges();
        }
    }
}
