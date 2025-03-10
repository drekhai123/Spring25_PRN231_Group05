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
using FlowerFarmTaskManagementSystem.BusinessObject.Enums;

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
                includeProperties: "User,TaskWork.ProductField.Product.Category,TaskWork.ProductField.Field"
            ));
            return _mapper.Map<IEnumerable<UserTaskResponseDTO>>(userTasks);
        }

        public async Task<UserTaskResponseDTO> GetUserTaskByIdAsync(Guid id)
        {
            var userTask = await Task.FromResult(_unitOfWork.UserTaskRepository.Get(
                filter: ut => ut.UserTaskId == id,
                includeProperties: "User,TaskWork.ProductField.Product.Category,TaskWork.ProductField.Field"
            ).FirstOrDefault());

            if (userTask == null)
                throw new KeyNotFoundException($"UserTask with ID {id} not found");

            return _mapper.Map<UserTaskResponseDTO>(userTask);
        }

        public async Task<IEnumerable<UserTaskResponseDTO>> GetUserTasksByUserIdAsync(Guid userId)
        {
            var userTasks = _unitOfWork.UserTaskRepository.Get(
                ut => ut.UserId == userId,
                includeProperties: "User,TaskWork.ProductField.Product.Category,TaskWork.ProductField.Field"
            );

            return _mapper.Map<IEnumerable<UserTaskResponseDTO>>(userTasks);
        }

        public async Task<UserTaskResponseDTO> CreateUserTaskAsync(UserTaskRequestDTO userTaskRequest)
        {
            // Validate if User and TaskWork exist
            //var user = await _unitOfWork.UserRepository.GetByIdAsync(userTaskRequest.UserId);
            //if (user == null)
            //    throw new KeyNotFoundException($"User with ID {userTaskRequest.UserId} not found");

            //var taskWork = await _unitOfWork.TaskWorkRepository.GetByIdAsync(userTaskRequest.TaskWorkId);
            //if (taskWork == null)
            //    throw new KeyNotFoundException($"TaskWork with ID {userTaskRequest.TaskWorkId} not found");

            var userTask = _mapper.Map<UserTask>(userTaskRequest);

            // Tạo FarmToolsOfTask cho mỗi FarmToolId
            if (userTaskRequest.FarmToolIds != null && userTaskRequest.FarmToolIds.Any())
            {
                foreach (var farmToolId in userTaskRequest.FarmToolIds)
                {
                    userTask.FarmToolsOfTasks.Add(new FarmToolsOfTask
                    {
                        FarmToolsId = farmToolId,
                        UserTaskId = userTask.UserTaskId
                    });
                }
            }

            await _unitOfWork.UserTaskRepository.AddAsync(userTask);
            await _unitOfWork.SaveChangesAsync();

            // Load lại userTask với đầy đủ thông tin
            var createdUserTask = await Task.FromResult(_unitOfWork.UserTaskRepository.Get(
                filter: ut => ut.UserTaskId == userTask.UserTaskId,
                includeProperties: "User,TaskWork.ProductField.Product.Category,TaskWork.ProductField.Field,FarmTools.FarmToolCategories"
            ).FirstOrDefault());

            return _mapper.Map<UserTaskResponseDTO>(createdUserTask);
        }

        public async Task<UserTaskResponseDTO> UpdateUserTaskAsync(Guid id, UserTaskRequestDTO userTaskRequest)
        {
            var userTask = await _unitOfWork.UserTaskRepository.GetByIdAsync(id);
            if (userTask == null)
                throw new KeyNotFoundException($"UserTask with ID {id} not found");

            userTask.Status = (int)userTaskRequest.Status;
            userTask.UpdateDate = DateTime.UtcNow;

            // Cập nhật FarmToolsId (lấy tool đầu tiên nếu có)
            //userTask.FarmToolsId = userTaskRequest.FarmToolIds.FirstOrDefault();

            _unitOfWork.UserTaskRepository.Update(userTask);
            await _unitOfWork.SaveChangesAsync();

            var updatedUserTask = await Task.FromResult(_unitOfWork.UserTaskRepository.Get(
                filter: ut => ut.UserTaskId == id,
                includeProperties: "User,TaskWork.ProductField.Product.Category,TaskWork.ProductField.Field,FarmTools.FarmToolCategories"
            ).FirstOrDefault());

            return _mapper.Map<UserTaskResponseDTO>(updatedUserTask);
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