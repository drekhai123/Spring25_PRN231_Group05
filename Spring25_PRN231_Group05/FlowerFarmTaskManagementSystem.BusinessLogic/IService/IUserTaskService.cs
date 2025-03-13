using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IUserTaskService
    {
        Task<IEnumerable<UserTaskResponseDTO>> GetAllUserTasksAsync();
        Task<UserTaskResponseDTO> GetUserTaskByIdAsync(Guid id);
        Task<IEnumerable<UserTaskResponseDTO>> GetUserTasksByUserIdAsync(Guid userId);
        Task<UserTaskResponseDTO> CreateUserTaskAsync(UserTaskRequestDTO userTaskRequest);
        Task<UserTaskResponseDTO> UpdateUserTaskAsync(Guid id, UserTaskRequestDTO userTaskRequest);
        Task<bool> DeleteUserTaskAsync(Guid id);
    }
}