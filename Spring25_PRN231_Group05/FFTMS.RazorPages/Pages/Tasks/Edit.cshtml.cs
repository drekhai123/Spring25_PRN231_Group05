using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private Guid _taskId;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskRequestDTO Task { get; set; } = default!;
        public string? ErrorMessage { get; set; }
        public SelectList UserList { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                // Get all users
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (userResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Filter users with role "User"
                    var usersList = users.Where(u => u.Role == "User")
                                      .Select(u => new SelectListItem
                                      {
                                          Value = u.Email,
                                          Text = $"{u.FirstName} {u.LastName} ({u.Email})"
                                      });

                    UserList = new SelectList(usersList, "Value", "Text");
                }

                // Get task details
                _taskId = id;
                var taskResponse = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{id}");
                if (taskResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await taskResponse.Content.ReadAsStringAsync();
                    var task = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Task = new TaskRequestDTO
                    {
                        JobTitle = task.JobTitle,
                        Description = task.Description,
                        AssignedTo = task.AssignedTo,
                        AssignedBy = task.AssignedBy,
                        StartDate = task.StartDate,
                        EndDate = task.EndDate,
                        Status = task.Status,
                        ImageUrl = task.ImageUrl
                    };

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

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7207/odata/Task/{id}", Task);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = $"Error updating task. Status code: {response.StatusCode}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating task: {ex.Message}";
                return Page();
            }
        }
    }

    public class UserDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}