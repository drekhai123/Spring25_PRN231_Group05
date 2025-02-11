using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> GetUserByIdAsync(Guid id);
        Task<UserResponseDTO> CreateUserAsync(UserRequestDTO userDto);
        Task<UserResponseDTO> UpdateUserAsync(Guid id, UserRequestDTO userDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserResponseDTO> UpdateUserStatusAsync(Guid id);
    }
}