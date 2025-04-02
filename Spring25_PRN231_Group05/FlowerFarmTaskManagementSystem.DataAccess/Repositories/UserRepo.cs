using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FlowerFarmTaskManagementSystem.DataAccess.Repositories
{
    public class UserRepo : GenericRepository<User>, IUser
    {
        private readonly FlowerFarmTaskManagementSystemDbContext _context;

        public UserRepo(FlowerFarmTaskManagementSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Only return active users by default
            return Get(u => u.IsActive).ToList();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.UserId = Guid.NewGuid();
            user.CreateDate = DateTime.Now;
            user.IsActive = true;

            await AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await GetByIdAsync(id);
            if (existingUser == null) return null;

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;
            existingUser.Role = user.Role;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.IsActive = user.IsActive;

            Update(existingUser);
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            // Soft delete - set IsActive to false instead of removing the record
            user.IsActive = false;
            Update(user);
            return true;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var users = Get(u => u.Email == email);
            return users.FirstOrDefault();
        }

        public async Task<User> UpdateUserStatusAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return null;

            user.IsActive = !user.IsActive;
            Update(user);
            return user;
        }
    }
}