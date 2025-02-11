using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using System.Text.RegularExpressions;
using AutoMapper;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class UserService : IUserService
    {
        private readonly IUser _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUser userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return _mapper.Map<UserResponseDTO>(user);
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

            var user = _mapper.Map<User>(userDto);
            user.IsActive = true; // Set default values

            var createdUser = await _userRepository.CreateUserAsync(user);
            return _mapper.Map<UserResponseDTO>(createdUser);
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

            _mapper.Map(userDto, currentUser);
            var updatedUser = await _userRepository.UpdateUserAsync(id, currentUser);
            return _mapper.Map<UserResponseDTO>(updatedUser);
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
            return _mapper.Map<UserResponseDTO>(user);
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