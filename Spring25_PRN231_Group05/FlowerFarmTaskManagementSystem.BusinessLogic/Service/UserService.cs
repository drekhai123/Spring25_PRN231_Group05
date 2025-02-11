using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
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

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToResponseDTO);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return MapToResponseDTO(user);
        }

        public async Task<UserResponseDTO> CreateUserAsync(UserRequestDTO userDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(userDto.Password))
                throw new ArgumentException("Password is required");

            // Validate email format
            //if (!IsValidEmail(userDto.Email))
            //    throw new ArgumentException("Invalid email format");

            // if (userDto.Password.Length < 6)
            //     throw new ArgumentException("Password must be at least 6 characters long");

            // Validate phone number if provided
            //if (!string.IsNullOrWhiteSpace(userDto.Phone) && !IsValidPhoneNumber(userDto.Phone))
            //    throw new ArgumentException("Invalid phone number format");

            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            var user = new User
            {
                UserName = userDto.UserName,
                Password = userDto.Password,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address,
                Role = userDto.Role,
                DateOfBirth = userDto.DateOfBirth,
                IsActive = true
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            return MapToResponseDTO(createdUser);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(Guid id, UserRequestDTO userDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(userDto.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(userDto.Role))
                throw new ArgumentException("Role is required");

            // Validate email format
            //if (!IsValidEmail(userDto.Email))
            //    throw new ArgumentException("Invalid email format");

            // Validate phone number if provided
            //if (!string.IsNullOrWhiteSpace(userDto.Phone) && !IsValidPhoneNumber(userDto.Phone))
            //    throw new ArgumentException("Invalid phone number format");

            // Check if email exists for different user
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null && existingUser.UserId != id)
                throw new ArgumentException("Email already exists for another user");

            // Lấy user hiện tại để giữ nguyên các giá trị cũ
            var currentUser = await _userRepository.GetUserByIdAsync(id);
            if (currentUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            // Cập nhật chỉ những thông tin từ request
            currentUser.UserName = userDto.UserName;
            currentUser.Email = userDto.Email;
            currentUser.Phone = userDto.Phone;
            currentUser.Address = userDto.Address;
            currentUser.Role = userDto.Role;
            currentUser.DateOfBirth = userDto.DateOfBirth;
            // Giữ nguyên IsActive từ dữ liệu cũ

            var updatedUser = await _userRepository.UpdateUserAsync(id, currentUser);
            return MapToResponseDTO(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return result;
        }

        public async Task<UserResponseDTO> UpdateUserStatusAsync(Guid id)
        {
            var user = await _userRepository.UpdateUserStatusAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return MapToResponseDTO(user);
        }

        private UserResponseDTO MapToResponseDTO(User user)
        {
            return new UserResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                CreateDate = user.CreateDate,
                IsActive = user.IsActive
            };
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