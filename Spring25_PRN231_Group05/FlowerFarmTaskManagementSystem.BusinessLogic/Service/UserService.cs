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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> CreateUserAsync(UserRequestDTO userDto)
        {
            ValidateUserData(userDto);

            // Check if email already exists
            var existingUser = await GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            var user = _mapper.Map<User>(userDto);
            user.IsActive = true;

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(Guid id, UserRequestDTO userDto)
        {
            ValidateUserData(userDto);

            // Check if email exists for different user
            var existingUser = await GetUserByEmailAsync(userDto.Email);
            if (existingUser != null && existingUser.UserId != id)
                throw new ArgumentException("Email already exists for another user");

            var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (currentUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            _mapper.Map(userDto, currentUser);
            _unitOfWork.UserRepository.Update(currentUser);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(currentUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponseDTO> UpdateUserStatusAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            user.IsActive = !user.IsActive;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        private async Task<User> GetUserByEmailAsync(string email)
        {
            var users = _unitOfWork.UserRepository.Get(u => u.Email == email);
            return users.FirstOrDefault();
        }

        private void ValidateUserData(UserRequestDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(userDto.Password))
                throw new ArgumentException("Password is required");
            if (string.IsNullOrWhiteSpace(userDto.Role))
                throw new ArgumentException("Role is required");

            // Tạm thời comment lại validation email và phone
            //if (!IsValidEmail(userDto.Email))
            //    throw new ArgumentException("Invalid email format");
            //if (!string.IsNullOrWhiteSpace(userDto.Phone) && !IsValidPhoneNumber(userDto.Phone))
            //    throw new ArgumentException("Invalid phone number format");
        }

        //private bool IsValidEmail(string email)
        //{
        //    try
        //    {
        //        var addr = new System.Net.Mail.MailAddress(email);
        //        return addr.Address == email;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool IsValidPhoneNumber(string phone)
        //{
        //    var regex = new Regex(@"^[0-9]{10,11}$");
        //    return regex.IsMatch(phone);
        //}
    }
}