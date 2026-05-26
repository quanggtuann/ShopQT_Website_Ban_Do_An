using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;
namespace ShopDAL.Areas.Repository
{
    public class AdminAccountRepo : IAdminAccountRepo
    {
        private readonly ApplicationDbContext _context;
        public AdminAccountRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public List<User> GetFiltered(string keyword, bool? isActive, string role, string sortBy, string sortOrder)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u => u.Username.Contains(keyword) ||
                                         u.Email.Contains(keyword) ||
                                         u.PhoneNumber.Contains(keyword));
            }
            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }
            var isDescending = sortOrder?.ToLower() == "desc";

            switch (sortBy?.ToLower())
            {
                case "username":
                    query = isDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username);
                    break;
                case "email":
                    query = isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                    break;
                case "role":
                    query = isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role);
                    break;
                case "status":
                    query = isDescending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive);
                    break;
                default:
                    query = isDescending ? query.OrderByDescending(u => u.UserID) : query.OrderBy(u => u.UserID);
                    break;
            }

            return query.ToList();
        }
        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserID == id);
        }
        private void validateUser(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username && u.UserID != user.UserID))
            {
                throw new InvalidOperationException($"username  '{user.Username}' is not valid");
            }
            if (_context.Users.Any(u => u.Email == user.Email && u.UserID != user.UserID))
            {
                throw new InvalidOperationException($"Email '{user.Email}' is not valid");
            }
            if (_context.Users.Any(u => u.PhoneNumber == user.PhoneNumber && u.UserID != user.UserID))
            {
                throw new InvalidOperationException($"PhoneNumber '{user.PhoneNumber}' is not valid");
            }
            if (user.Role != "customer" && user.Role != "admin")
            {
                throw new InvalidOperationException($"the role '{user.Role}' is not valid");
            }
        }
        public void Update(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == user.UserID);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found");
            }
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateorBirth = user.DateorBirth;
            existingUser.IsActive = user.IsActive;
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = user.Password;
            }
            validateUpdate(existingUser);
            _context.SaveChanges();
        }

        private void validateUpdate(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email && u.UserID != user.UserID))
            {
                throw new InvalidOperationException($"Email '{user.Email}' is already in use");
            }
            if (_context.Users.Any(u => u.PhoneNumber == user.PhoneNumber && u.UserID != user.UserID))
            {
                throw new InvalidOperationException($"PhoneNumber '{user.PhoneNumber}' is already in use");
            }
        }
        public void Add(User user)
        {
            validateUser(user);
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void Deactive(int id)
        {
            var us = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (us != null)
            {
                us.IsActive = false;
                _context.SaveChanges();
            }
        }
        public void ActiveUser(int id)
        {
            var us = _context.Users.FirstOrDefault(u=>u.UserID == id);
            if(us != null)
            {
                us.IsActive = true;
                _context.SaveChanges();
            }
        }
    }
}

