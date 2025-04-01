using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<TaskResponseDTO>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDTO>> GetTaskById(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDTO>> CreateTask([FromBody] TaskRequestDTO taskRequest)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(taskRequest);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskWorkId }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDTO>> UpdateTask(Guid id, [FromBody] TaskRequestDTO taskRequest)
        {
            try
            {
                var task = await _taskService.UpdateTaskAsync(id, taskRequest);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTask(Guid id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                return Ok(new { Success = result, Message = "Task has been deactivated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred" });
            }
        }

        [HttpGet("GetTasksByUserId")]
        public async Task<ActionResult<IEnumerable<object>>> GetTasksByUserId([FromQuery] string userId, [FromQuery] string excludeTaskId = null)
        {
            try
            {
                var allTasks = await _taskService.GetAllTasksAsync();
                
                // Filter tasks for the specific user
                var userTasks = allTasks
                    .Where(t => t.UserTasks != null && 
                                t.UserTasks.Any(ut => ut.UserId.ToString() == userId) && 
                                t.Status == true)
                    .ToList();
                
                // Exclude a specific task if needed (for Edit page)
                if (!string.IsNullOrEmpty(excludeTaskId))
                {
                    var taskIdToExclude = Guid.Parse(excludeTaskId);
                    userTasks = userTasks.Where(t => t.TaskWorkId != taskIdToExclude).ToList();
                }
                
                // Simplify and transform the data for the UI
                var simplifiedTasks = userTasks.Select(t => new
                {
                    taskId = t.TaskWorkId,
                    jobTitle = t.JobTitle,
                    description = t.Description,
                    startDate = t.StartDate,
                    endDate = t.EndDate,
                    status = t.UserTasks.First(ut => ut.UserId.ToString() == userId).Status,
                    taskStatus = t.TaskStatus
                }).ToList();
                
                return Ok(simplifiedTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}