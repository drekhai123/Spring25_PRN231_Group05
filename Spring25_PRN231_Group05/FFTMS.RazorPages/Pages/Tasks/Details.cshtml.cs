using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Collections.Generic;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public TaskResponseDTO Task { get; set; } = default!;
        public ProductFieldResponse? ProductField { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Auth/LoginPage");
                }

                var role = JwtHelper.GetRoleFromToken(token);
                if (role != "Manager")
                {
                    return RedirectToPage("/Index");
                }

                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Task = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    // Initialize default values for potentially null properties
                    if (Task == null)
                    {
                        Task = new TaskResponseDTO
                        {
                            JobTitle = "N/A",
                            Description = "N/A",
                            UserTasks = new List<UserTaskResponseDTO>(),
                            Productivity = 0,
                            ProductivityUnit = "N/A"
                        };
                    }
                    else
                    {
                        // Initialize Product if it's null
                        if (Task.Product == null)
                        {
                            Task.Product = new ProductDTO 
                            { 
                                ProductName = "N/A",
                                Category = new CategoryResponseDTO { CategoryName = "N/A" }
                            };
                        }
                        else if (Task.Product.Category == null)
                        {
                            Task.Product.Category = new CategoryResponseDTO { CategoryName = "N/A" };
                        }
                        
                        // Initialize Field if it's null
                        if (Task.Field == null)
                        {
                            Task.Field = new FieldDTO
                            {
                                FieldName = "N/A"
                            };
                        }
                        
                        // Ensure UserTasks is initialized
                        if (Task.UserTasks == null)
                        {
                            Task.UserTasks = new List<UserTaskResponseDTO>();
                        }
                        
                        // Set default values for Productivity and ProductivityUnit if necessary
                        if (Task.Productivity <= 0)
                        {
                            Task.Productivity = 0;
                        }
                        
                        if (string.IsNullOrEmpty(Task.ProductivityUnit))
                        {
                            Task.ProductivityUnit = "N/A";
                        }

                        // Fetch ProductField data if ProductFieldId exists
                        if (Task.ProductFieldId != Guid.Empty)
                        {
                            var productFieldResponse = await _httpClient.GetAsync($"http://localhost:5281/odata/ProductField/{Task.ProductFieldId}");
                            if (productFieldResponse.IsSuccessStatusCode)
                            {
                                var productFieldJson = await productFieldResponse.Content.ReadAsStringAsync();
                                ProductField = JsonSerializer.Deserialize<ProductFieldResponse>(productFieldJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                            }
                        }
                        
                        // Check and update Task Status if all staff assignments are completed
                        if (Task.UserTasks.Count > 0 && Task.UserTasks.All(ut => Convert.ToInt32(ut.Status) == 2))
                        {
                            if (Task.TaskStatus != TaskProgressStatus.COMPLETED)
                            {
                                // Update task status to completed in database
                                await UpdateTaskStatusToCompleted(Task.TaskWorkId);
                                // Update local task object
                                Task.TaskStatus = TaskProgressStatus.COMPLETED;
                            }
                        }
                    }
                    
                    return Page();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading task: {ex.Message}";
                return Page();
            }
        }
        
        private async Task UpdateTaskStatusToCompleted(Guid taskId)
        {
            try
            {
                // Get the current task to preserve its data
                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{taskId}");
                if (!response.IsSuccessStatusCode) return;
                
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var taskToUpdate = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (taskToUpdate == null) return;
                
                // Create request DTO for update
                var updateRequest = new
                {
                    JobTitle = taskToUpdate.JobTitle,
                    Description = taskToUpdate.Description,
                    AssignedBy = taskToUpdate.AssignedBy,
                    StartDate = taskToUpdate.StartDate,
                    EndDate = taskToUpdate.EndDate,
                    Status = taskToUpdate.Status,
                    ImageUrl = taskToUpdate.ImageUrl,
                    ProductFieldId = taskToUpdate.ProductFieldId,
                    TaskStatus = (int)TaskProgressStatus.COMPLETED,
                    UserTasks = taskToUpdate.UserTasks.Select(ut => new 
                    {
                        AssignedTo = ut.UserId.ToString(),
                        UserTaskDescription = ut.UserTaskDescription
                    }).ToList()
                };
                
                // Update the task
                var updateResponse = await _httpClient.PutAsJsonAsync($"https://localhost:7207/odata/Task/{taskId}", updateRequest);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    var errorContent = await updateResponse.Content.ReadAsStringAsync();
                    ErrorMessage = $"Error updating task status: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating task status: {ex.Message}";
            }
        }
    }
}