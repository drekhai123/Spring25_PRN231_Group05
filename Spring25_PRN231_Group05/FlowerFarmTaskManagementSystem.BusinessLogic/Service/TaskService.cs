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
            task.ProductFieldId = taskRequest.ProductFieldId;
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
            await ValidateTaskData(taskRequest, id);

            var task = await _unitOfWork.TaskWorkRepository.GetByIdAsync(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            // Cập nhật thông tin task
            _mapper.Map(taskRequest, task);
            task.ProductFieldId = taskRequest.ProductFieldId;

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
            var task = await Task.FromResult(_unitOfWork.TaskWorkRepository.Get(
                filter: t => t.TaskWorkId == id,
                includeProperties: "UserTasks,ProductField.Product.Category"
            ).FirstOrDefault());

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            // Validate trước khi vô hiệu hóa
            await ValidateTaskDeletion(task);

            // Thay đổi status thành false thay vì xóa
            task.Status = false;
            _unitOfWork.TaskWorkRepository.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task ValidateTaskData(TaskRequestDTO taskRequest, Guid? taskId = null)
        {
            if (string.IsNullOrWhiteSpace(taskRequest.JobTitle))
                throw new ArgumentException("Job title is required");
            if (string.IsNullOrWhiteSpace(taskRequest.Description))
                throw new ArgumentException("Description is required");
            
            // First check: End date must not be earlier than start date
            if (taskRequest.EndDate.Date < taskRequest.StartDate.Date)
            {
                throw new ArgumentException("Start date must be before end date");
            }
            
            // Second check: If on same day, check if time difference is at least 2 hours
            TimeSpan timeDifference = taskRequest.EndDate - taskRequest.StartDate;
            if (timeDifference.TotalHours < 2)
            {
                throw new ArgumentException("End time must be at least 2 hours after start time");
            }

            // Validate ProductField và các bảng liên quan
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(taskRequest.ProductFieldId);
            if (productField == null)
                throw new ArgumentException($"ProductField with ID {taskRequest.ProductFieldId} not found");

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

            // Kiểm tra tất cả user được assign và kiểm tra trùng lặp thời gian
            foreach (var userTask in taskRequest.UserTasks)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(userTask.AssignedTo));
                if (user == null)
                    throw new ArgumentException($"User with ID {userTask.AssignedTo} not found");
                if (!user.IsActive)
                    throw new ArgumentException($"User with ID {userTask.AssignedTo} is inactive");
                if (string.IsNullOrWhiteSpace(userTask.UserTaskDescription))
                    throw new ArgumentException($"Task description for user {userTask.AssignedTo} is required");

                // Kiểm tra trùng lặp thời gian
                await CheckTimeOverlapForUser(Guid.Parse(userTask.AssignedTo), taskRequest.StartDate, taskRequest.EndDate, taskId);
            }
        }

        private async Task CheckTimeOverlapForUser(Guid userId, DateTime startDate, DateTime endDate, Guid? currentTaskId = null)
        {
            // Lấy thông tin người dùng
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            // Lấy tất cả task mà user tham gia (chỉ lấy các task có status = true và user task status = 0 hoặc 1)
            var userTasks = await Task.FromResult(_unitOfWork.UserTaskRepository.Get(
                filter: ut => ut.UserId == userId && 
                              (ut.Status == (int)UserTaskStatus.Waiting || ut.Status == (int)UserTaskStatus.Processing) &&
                              ut.TaskWork.Status == true,
                includeProperties: "TaskWork"
            ));

            // Nếu đang cập nhật task hiện tại, loại bỏ chính nó khỏi danh sách kiểm tra
            if (currentTaskId.HasValue)
            {
                userTasks = userTasks.Where(ut => ut.TaskWorkId != currentTaskId.Value).ToList();
            }

            foreach (var userTask in userTasks)
            {
                var taskWork = userTask.TaskWork;
                
                // Kiểm tra xem có trùng lặp thời gian không
                // Trùng lặp xảy ra khi một trong các điều kiện sau đúng:
                // 1. startDate nằm trong khoảng thời gian của task hiện có
                // 2. endDate nằm trong khoảng thời gian của task hiện có
                // 3. Task mới bao trùm toàn bộ khoảng thời gian của task hiện có
                if ((startDate >= taskWork.StartDate && startDate < taskWork.EndDate) ||
                    (endDate > taskWork.StartDate && endDate <= taskWork.EndDate) ||
                    (startDate <= taskWork.StartDate && endDate >= taskWork.EndDate))
                {
                    throw new ArgumentException(
                        $"User '{user.UserName}' already has a task assigned during this time period.\n" +
                        $"Conflicting Task: {taskWork.JobTitle}\n" +
                        $"Time Period: {taskWork.StartDate.ToString("dd/MM/yyyy HH:mm")} - {taskWork.EndDate.ToString("dd/MM/yyyy HH:mm")}\n" +
                        "Please select a different time period or assign the task to another user."
                    );
                }
            }
        }

        private async Task ValidateTaskDeletion(TaskWork task)
        {
            // Kiểm tra trạng thái của task
            if (!task.Status)
                throw new InvalidOperationException("Cannot delete an inactive task");

            // Kiểm tra trạng thái của các UserTask
            // foreach (var userTask in task.UserTasks)
            // {
            //     if (userTask.Status == (int)UserTaskStatus.Processing)
            //         throw new InvalidOperationException("Cannot delete task because it has tasks in progress");
            //     if (userTask.Status == (int)UserTaskStatus.Completed)
            //         throw new InvalidOperationException("Cannot delete task because it has completed tasks");
            // }

            // Kiểm tra ProductField
            if (task.ProductField != null)
            {
                // Kiểm tra Product
                if (task.ProductField.Product != null && !task.ProductField.Product.Status)
                    throw new InvalidOperationException("Cannot delete task because associated product is inactive");

                // Kiểm tra Category
                if (task.ProductField.Product?.Category != null && !task.ProductField.Product.Category.Status)
                    throw new InvalidOperationException("Cannot delete task because associated category is inactive");
            }
        }
    }
}