using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskRequestDTO Task { get; set; } = new TaskRequestDTO();
        public SelectList UserList { get; set; }
        public SelectList ProductFieldList { get; set; }
        public string ErrorMessage { get; set; }
        public Guid TaskId { get; set; }
        public List<ProductFieldResponse> ProductFieldsData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
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

                var userId = JwtHelper.GetUserIdFromToken(token);

                TaskId = Guid.Parse(id);
                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var taskResponse = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    // Check if task is already completed, if so redirect to details page
                    if (taskResponse.TaskStatus == TaskProgressStatus.COMPLETED)
                    {
                        TempData["ErrorMessage"] = "Cannot edit a completed task.";
                        return RedirectToPage("./Details", new { id = TaskId });
                    }

                    Task = new TaskRequestDTO
                    {
                        JobTitle = taskResponse.JobTitle,
                        Description = taskResponse.Description,
                        AssignedBy = userId,
                        StartDate = taskResponse.StartDate,
                        EndDate = taskResponse.EndDate,
                        Status = taskResponse.Status,
                        ImageUrl = taskResponse.ImageUrl,
                        ProductFieldId = taskResponse.ProductFieldId,
                        UserTasks = taskResponse.UserTasks?.Select(ut => new UserTaskRequest
                        {
                            AssignedTo = ut.UserId.ToString(),
                            UserTaskDescription = ut.UserTaskDescription
                        }).ToList() ?? new List<UserTaskRequest>()
                    };

                    await LoadUserList();
                    await LoadProductFieldList();
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

        private async Task LoadUserList()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var usersList = users.Where(u => u.Role == "Staff")
                                      .Select(u => new SelectListItem
                                      {
                                          Value = u.UserId.ToString(),
                                          Text = $"{u.UserName} ({u.Email})"
                                      });

                    UserList = new SelectList(usersList, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading users: {ex.Message}";
            }
        }

        private async Task LoadProductFieldList()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5281/odata/ProductFields/get-all-productField");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var productFields = JsonSerializer.Deserialize<List<ProductFieldResponse>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ProductFieldsData = productFields;

                    var productFieldsList = productFields.Select(pf => new SelectListItem
                    {
                        Value = pf.ProductFieldId.ToString(),
                        Text = $"{pf.Product.ProductName} - {pf.ProductivityUnit}"
                    });

                    ProductFieldList = new SelectList(productFieldsList, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading product fields: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Auth/LoginPage");
                }

                // Lấy userId trực tiếp từ helper
                var userId = JwtHelper.GetUserIdFromToken(token);
                Task.AssignedBy = userId;

                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7207/odata/Task/{id}", Task);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = responseContent ?? "Error updating task";
                await LoadUserList();
                await LoadProductFieldList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await LoadUserList();
                await LoadProductFieldList();
                return Page();
            }
        }
    }
}