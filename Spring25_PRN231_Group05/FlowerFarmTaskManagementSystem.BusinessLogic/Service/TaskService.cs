using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using FlowerFarmTaskManagementSystem.BusinessObject.Enums;

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
            var tasks = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                includeProperties: "UserTasks.User,ProductField.Product.Category,ProductField.Field"
            ));
            return _mapper.Map<IEnumerable<TaskResponseDTO>>(tasks);
        }

        public async Task<TaskResponseDTO> GetTaskByIdAsync(Guid id)
        {
            var task = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                filter: t => t.TaskWorkId == id,
                includeProperties: "UserTasks.User,ProductField.Product.Category,ProductField.Field"
            ).FirstOrDefault());

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            return _mapper.Map<TaskResponseDTO>(task);
        }

        public async Task<TaskResponseDTO> CreateTaskAsync(TaskRequestDTO taskRequest)
        {
            await ValidateTaskData(taskRequest);

            // Tạo Task mới
            var task = _mapper.Map<TaskWork>(taskRequest);
            task.TaskWorkId = Guid.NewGuid();
            task.CreateDate = DateTime.UtcNow;
            task.Status = true;
            task.ProductFieldId = taskRequest.ProductField.ProductFieldId;
            task.AssignedBy = taskRequest.AssignedBy;

            // Tạo danh sách UserTask cho từng staff
            var userTasks = taskRequest.UserTasks.Select(userTask => new UserTask
            {
                UserTaskId = Guid.NewGuid(),
                TaskWorkId = task.TaskWorkId,
                UserId = Guid.Parse(userTask.AssignedTo),
                UserTaskDescription = userTask.UserTaskDescription,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Status = (int)UserTaskStatus.Waiting
            }).ToList();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Chỉ lưu TaskWork và UserTasks, không lưu ProductField
                    await _unitOfWork.TaskWorkRepository.AddAsync(task);

                    foreach (var userTask in userTasks)
                    {
                        await _unitOfWork.UserTaskRepository.AddAsync(userTask);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Load lại task với đầy đủ thông tin
                    var createdTask = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                        filter: t => t.TaskWorkId == task.TaskWorkId,
                        includeProperties: "UserTasks.User,ProductField.Product.Category,ProductField.Field"
                    ).FirstOrDefault());

                    await transaction.CommitAsync();
                    return _mapper.Map<TaskResponseDTO>(createdTask);
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    throw;
                }
            }
        }

        public async Task<TaskResponseDTO> UpdateTaskAsync(Guid id, TaskRequestDTO taskRequest)
        {
            await ValidateTaskData(taskRequest);

            var task = await _unitOfWork.TaskWorkRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            // Cập nhật thông tin task
            task.JobTitle = taskRequest.JobTitle;
            task.Description = taskRequest.Description;
            task.AssignedBy = taskRequest.AssignedBy;
            task.StartDate = taskRequest.StartDate;
            task.EndDate = taskRequest.EndDate;
            task.Status = taskRequest.Status;
            task.ImageUrl = taskRequest.ImageUrl;
            task.ProductFieldId = taskRequest.ProductField.ProductFieldId;

            // Tạo danh sách UserTask mới
            var userTasks = taskRequest.UserTasks.Select(userTask => new UserTask
            {
                UserTaskId = Guid.NewGuid(),
                TaskWorkId = task.TaskWorkId,
                UserId = Guid.Parse(userTask.AssignedTo),
                UserTaskDescription = userTask.UserTaskDescription,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Status = (int)UserTaskStatus.Waiting
            }).ToList();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Cập nhật task
                    _unitOfWork.TaskWorkRepository.Update(task);

                    // Xóa các UserTask cũ
                    var oldUserTasks = _unitOfWork.UserTaskRepository.Get(
                        filter: ut => ut.TaskWorkId == id
                    );
                    foreach (var oldUserTask in oldUserTasks)
                    {
                        _unitOfWork.UserTaskRepository.Delete(oldUserTask);
                    }

                    // Thêm các UserTask mới
                    foreach (var userTask in userTasks)
                    {
                        await _unitOfWork.UserTaskRepository.AddAsync(userTask);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Load lại task với đầy đủ thông tin
                    var updatedTask = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                        filter: t => t.TaskWorkId == task.TaskWorkId,
                        includeProperties: "UserTasks.User,ProductField.Product.Category,ProductField.Field"
                    ).FirstOrDefault());

                    await transaction.CommitAsync();
                    return _mapper.Map<TaskResponseDTO>(updatedTask);
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    throw;
                }
            }
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            // Kiểm tra task có tồn tại không
            var task = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                filter: t => t.TaskWorkId == id,
                includeProperties: "UserTasks,ProductField.Product.Category"
            ).FirstOrDefault());

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            // Validate trước khi xóa
            await ValidateTaskDeletion(task);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Xóa tất cả UserTask liên quan
                    foreach (var userTask in task.UserTasks)
                    {
                        _unitOfWork.UserTaskRepository.Delete(userTask);
                    }

                    // Xóa Task
                    _unitOfWork.TaskWorkRepository.Delete(task);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    throw;
                }
            }
        }

        private async Task ValidateTaskData(TaskRequestDTO taskRequest)
        {
            if (string.IsNullOrWhiteSpace(taskRequest.JobTitle))
                throw new ArgumentException("Job title is required");
            if (string.IsNullOrWhiteSpace(taskRequest.Description))
                throw new ArgumentException("Description is required");
            if (string.IsNullOrWhiteSpace(taskRequest.AssignedBy))
                throw new ArgumentException("AssignedBy is required");
            if (taskRequest.StartDate >= taskRequest.EndDate)
                throw new ArgumentException("Start date must be before end date");

            // Validate ProductField và các bảng liên quan
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(taskRequest.ProductField.ProductFieldId);
            if (productField == null)
                throw new ArgumentException($"ProductField with ID {taskRequest.ProductField.ProductFieldId} not found");

            // Kiểm tra Product
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productField.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productField.ProductId} not found");
            if (!product.Status)
                throw new ArgumentException($"Product with ID {productField.ProductId} is inactive");

            // Kiểm tra Field
            var field = await _unitOfWork.FieldRepository.GetByIdAsync(productField.FieldId);
            if (field == null)
                throw new ArgumentException($"Field with ID {productField.FieldId} not found");
            if (!field.Status)
                throw new ArgumentException($"Field with ID {productField.FieldId} is inactive");

            // Kiểm tra Category
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
                throw new ArgumentException($"Category with ID {product.CategoryId} not found");
            if (!category.Status)
                throw new ArgumentException($"Category with ID {product.CategoryId} is inactive");

            // Validate số lượng UserTask
            if (!taskRequest.UserTasks.Any())
                throw new ArgumentException("At least one staff assignment is required");
            if (taskRequest.UserTasks.Count > 5)
                throw new ArgumentException("Maximum 5 staff assignments allowed");

            // Kiểm tra tất cả user được assign
            foreach (var userTask in taskRequest.UserTasks)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(userTask.AssignedTo));
                if (user == null)
                    throw new ArgumentException($"User with ID {userTask.AssignedTo} not found");
                if (!user.IsActive)
                    throw new ArgumentException($"User with ID {userTask.AssignedTo} is inactive");
                if (string.IsNullOrWhiteSpace(userTask.UserTaskDescription))
                    throw new ArgumentException($"Task description for user {userTask.AssignedTo} is required");
            }
        }

        private async Task ValidateTaskDeletion(TaskWork task)
        {
            // Kiểm tra trạng thái của task
            if (!task.Status)
                throw new InvalidOperationException($"Task with ID {task.TaskWorkId} is already inactive");

            // Kiểm tra quyền xóa (ví dụ: chỉ Manager mới được xóa)
            // Có thể thêm logic kiểm tra quyền ở đây

            // Kiểm tra trạng thái của các UserTask
            foreach (var userTask in task.UserTasks)
            {
                if (userTask.Status == (int)UserTaskStatus.Processing)
                    throw new InvalidOperationException($"Cannot delete task because UserTask {userTask.UserTaskId} is in processing state");
                if (userTask.Status == (int)UserTaskStatus.Completed)
                    throw new InvalidOperationException($"Cannot delete task because UserTask {userTask.UserTaskId} is already completed");
            }

            // Kiểm tra ProductField
            if (task.ProductField != null)
            {
                // Kiểm tra Product
                if (task.ProductField.Product != null && !task.ProductField.Product.Status)
                    throw new InvalidOperationException($"Cannot delete task because associated Product is inactive");

                // Kiểm tra Category
                if (task.ProductField.Product?.Category != null && !task.ProductField.Product.Category.Status)
                    throw new InvalidOperationException($"Cannot delete task because associated Category is inactive");
            }

            // Có thể thêm các validation khác tùy theo yêu cầu nghiệp vụ
            // Ví dụ: kiểm tra thời gian, kiểm tra dependencies, etc.
        }
    }
}