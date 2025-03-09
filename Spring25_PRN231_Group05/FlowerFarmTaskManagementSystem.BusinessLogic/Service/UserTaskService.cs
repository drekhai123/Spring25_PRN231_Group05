using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class UserTaskService : IUserTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserTaskResponseDTO>> GetAllUserTasksAsync()
        {
            var userTasks = await Task.FromResult(_unitOfWork.UserTaskRepository.Get(
                includeProperties: "User"
            ));
            return _mapper.Map<IEnumerable<UserTaskResponseDTO>>(userTasks);
        }

        public async Task<UserTaskResponseDTO> GetUserTaskByIdAsync(Guid id)
        {
            var userTask = await _unitOfWork.UserTaskRepository.GetByIdAsync(id);
            if (userTask == null)
                throw new KeyNotFoundException($"UserTask with ID {id} not found");
            return _mapper.Map<UserTaskResponseDTO>(userTask);
        }

        public async Task<IEnumerable<UserTaskResponseDTO>> GetUserTasksByUserIdAsync(Guid userId)
        {
            var userTasks = _unitOfWork.UserTaskRepository.Get(ut => ut.UserId == userId);
            return _mapper.Map<IEnumerable<UserTaskResponseDTO>>(userTasks);
        }

        public async Task<UserTaskResponseDTO> CreateUserTaskAsync(UserTaskRequestDTO userTaskRequest)
        {
            // Validate if User and TaskWork exist
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userTaskRequest.UserId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userTaskRequest.UserId} not found");

            var taskWork = await _unitOfWork.TaskWorkRepository.GetByIdAsync(userTaskRequest.TaskWorkId);
            if (taskWork == null)
                throw new KeyNotFoundException($"TaskWork with ID {userTaskRequest.TaskWorkId} not found");

            var userTask = _mapper.Map<UserTask>(userTaskRequest);
            userTask.CreateDate = DateTime.UtcNow;
            userTask.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.UserTaskRepository.AddAsync(userTask);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserTaskResponseDTO>(userTask);
        }

        public async Task<UserTaskResponseDTO> UpdateUserTaskAsync(Guid id, UserTaskRequestDTO userTaskRequest)
        {
            var userTask = await _unitOfWork.UserTaskRepository.GetByIdAsync(id);
            if (userTask == null)
                throw new KeyNotFoundException($"UserTask with ID {id} not found");

            // Validate if new User and TaskWork exist
            if (userTask.UserId != userTaskRequest.UserId)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userTaskRequest.UserId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userTaskRequest.UserId} not found");
            }

            if (userTask.TaskWorkId != userTaskRequest.TaskWorkId)
            {
                var taskWork = await _unitOfWork.TaskWorkRepository.GetByIdAsync(userTaskRequest.TaskWorkId);
                if (taskWork == null)
                    throw new KeyNotFoundException($"TaskWork with ID {userTaskRequest.TaskWorkId} not found");
            }

            _mapper.Map(userTaskRequest, userTask);
            userTask.UpdateDate = DateTime.UtcNow;

            _unitOfWork.UserTaskRepository.Update(userTask);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserTaskResponseDTO>(userTask);
        }

        public async Task<bool> DeleteUserTaskAsync(Guid id)
        {
            var userTask = await _unitOfWork.UserTaskRepository.GetByIdAsync(id);
            if (userTask == null)
                throw new KeyNotFoundException($"UserTask with ID {id} not found");

            _unitOfWork.UserTaskRepository.Delete(userTask);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}