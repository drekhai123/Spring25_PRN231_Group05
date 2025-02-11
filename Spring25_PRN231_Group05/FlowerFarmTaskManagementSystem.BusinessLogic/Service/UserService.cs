using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using System.Text.RegularExpressions;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class UserService : IUserService
    {
        private readonly IUser _userRepository;

        public UserService(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return user;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password is required");

            // Validate email format
            //if (!IsValidEmail(user.Email))
            //    throw new ArgumentException("Invalid email format");

            // if (user.Password.Length < 6)
            //     throw new ArgumentException("Password must be at least 6 characters long");

            // Validate phone number if provided
            //if (!string.IsNullOrWhiteSpace(user.Phone) && !IsValidPhoneNumber(user.Phone))
            //    throw new ArgumentException("Invalid phone number format");

            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(Guid id, User user)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("Role is required");

            // Validate email format
            //if (!IsValidEmail(user.Email))
            //    throw new ArgumentException("Invalid email format");

            // Validate phone number if provided
            //if (!string.IsNullOrWhiteSpace(user.Phone) && !IsValidPhoneNumber(user.Phone))
            //    throw new ArgumentException("Invalid phone number format");


            // Check if email exists for different user
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null && existingUser.UserId != id)
                throw new ArgumentException("Email already exists for another user");

            var updatedUser = await _userRepository.UpdateUserAsync(id, user);
            if (updatedUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return updatedUser;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return result;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            // Adjust regex pattern based on your phone number format requirements
            var regex = new Regex(@"^[0-9]{10,11}$");
            return regex.IsMatch(phone);
        }
    }
}