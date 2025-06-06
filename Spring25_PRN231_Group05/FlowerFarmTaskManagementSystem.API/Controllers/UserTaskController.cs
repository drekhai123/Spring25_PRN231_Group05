﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessObject.Enums;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly IMapper _mapper;

        public UserTaskController(IUserTaskService userTaskService, IMapper mapper)
        {
            _userTaskService = userTaskService;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<UserTaskResponseDTO>>> GetAllUserTasks()
        {
            var userTasks = await _userTaskService.GetAllUserTasksAsync();
            return Ok(userTasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserTaskResponseDTO>> GetUserTaskById(Guid id)
        {
            try
            {
                var userTask = await _userTaskService.GetUserTaskByIdAsync(id);
                return Ok(userTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserTaskResponseDTO>>> GetUserTasksByUserId(Guid userId)
        {
            var userTasks = await _userTaskService.GetUserTasksByUserIdAsync(userId);
            return Ok(userTasks);
        }

        [HttpPost]
        public async Task<ActionResult<UserTaskResponseDTO>> CreateUserTask([FromBody] UserTaskRequestDTO userTaskRequest)
        {
            try
            {
                var userTask = await _userTaskService.CreateUserTaskAsync(userTaskRequest);
                return CreatedAtAction(nameof(GetUserTaskById), new { id = userTask.UserTaskId }, userTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserTaskResponseDTO>> UpdateUserTask(Guid id, [FromBody] UserTaskRequestDTO userTaskRequest)
        {
            try
            {
                var userTask = await _userTaskService.UpdateUserTaskAsync(id, userTaskRequest);
                return Ok(userTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteUserTask(Guid id)
        {
            try
            {
                var result = await _userTaskService.DeleteUserTaskAsync(id);
                return Ok(new { Success = result, Message = "UserTask deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UserTaskStatusUpdateRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid ID");

                // Create UserTaskRequestDTO from request
                var userTaskRequest = new UserTaskRequestDTO
                {
                    Status = (UserTaskStatus)request.Status,
                    ImageUrl = request.ImageUrl
                };

                var updatedUserTask = await _userTaskService.UpdateUserTaskAsync(id, userTaskRequest);

                // Get the parent task to check if all user tasks are completed or processing
                var userTask = await _userTaskService.GetUserTaskByIdAsync(id);
                if (userTask != null && userTask.TaskWorkId != Guid.Empty)
                {
                    // Check if all user tasks for this task are completed
                    var taskId = userTask.TaskWorkId;
                    var taskService = HttpContext.RequestServices.GetService<ITaskService>();
                    if (taskService != null)
                    {
                        // Lấy task hiện tại để kiểm tra trạng thái
                        var task = await taskService.GetTaskByIdAsync(taskId);

                        // Đoạn này sẽ được xử lý trong GetTaskByIdAsync nên không cần code thêm ở đây
                        // GetTaskByIdAsync sẽ gọi CheckAndUpdateTaskCompletionStatus để cập nhật trạng thái của task và productField
                    }
                }

                return Ok(new { Message = "Task status updated successfully", UserTask = updatedUserTask });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error updating task status: {ex.Message}" });
            }
        }

        public class UpdateStatusDTO
        {
            public int Status { get; set; }
        }
    }
}