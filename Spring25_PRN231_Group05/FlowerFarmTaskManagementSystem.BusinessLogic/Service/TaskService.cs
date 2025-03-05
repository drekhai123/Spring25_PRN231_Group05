using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskResponseDTO>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.TaskWorkRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskResponseDTO>>(tasks);
        }

        public async Task<TaskResponseDTO> GetTaskByIdAsync(Guid id)
        {
            var task = await _unitOfWork.TaskWorkRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");
            return _mapper.Map<TaskResponseDTO>(task);
        }

        public async Task<TaskResponseDTO> CreateTaskAsync(TaskRequestDTO taskRequest)
        {
            ValidateTaskData(taskRequest);

            var task = _mapper.Map<TaskWork>(taskRequest);
            task.CreateDate = DateTime.UtcNow;
            task.Status = true;

            await _unitOfWork.TaskWorkRepository.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskResponseDTO>(task);
        }

        public async Task<TaskResponseDTO> UpdateTaskAsync(Guid id, TaskRequestDTO taskRequest)
        {
            ValidateTaskData(taskRequest);

            var task = await _unitOfWork.TaskWorkRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            _mapper.Map(taskRequest, task);
            _unitOfWork.TaskWorkRepository.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskResponseDTO>(task);
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = await _unitOfWork.TaskWorkRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            _unitOfWork.TaskWorkRepository.Delete(task);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateTaskData(TaskRequestDTO taskRequest)
        {
            if (string.IsNullOrWhiteSpace(taskRequest.JobTitle))
                throw new ArgumentException("Job title is required");
            if (string.IsNullOrWhiteSpace(taskRequest.Description))
                throw new ArgumentException("Description is required");
            if (string.IsNullOrWhiteSpace(taskRequest.AssignedTo))
                throw new ArgumentException("AssignedTo is required");
            if (string.IsNullOrWhiteSpace(taskRequest.AssignedBy))
                throw new ArgumentException("AssignedBy is required");
            if (taskRequest.StartDate >= taskRequest.EndDate)
                throw new ArgumentException("Start date must be before end date");
        }
    }
}