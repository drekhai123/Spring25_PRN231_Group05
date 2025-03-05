using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDTO>> GetAllTasksAsync();
        Task<TaskResponseDTO> GetTaskByIdAsync(Guid id);
        Task<TaskResponseDTO> CreateTaskAsync(TaskRequestDTO taskRequest);
        Task<TaskResponseDTO> UpdateTaskAsync(Guid id, TaskRequestDTO taskRequest);
        Task<bool> DeleteTaskAsync(Guid id);
    }
}