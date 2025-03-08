using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;

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
        public string ErrorMessage { get; set; }
        public Guid TaskId { get; set; }

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

                TaskId = Guid.Parse(id);
                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var taskResponse = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Map từ TaskResponseDTO sang TaskRequestDTO
                    Task = new TaskRequestDTO
                    {
                        JobTitle = taskResponse.JobTitle,
                        Description = taskResponse.Description,
                        AssignedTo = taskResponse.AssignedTo,
                        AssignedBy = taskResponse.AssignedBy,
                        StartDate = taskResponse.StartDate,
                        EndDate = taskResponse.EndDate,
                        Status = taskResponse.Status,
                        ImageUrl = taskResponse.ImageUrl
                    };

                    // Load users cho dropdown
                    await LoadUserList();
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
            var token = Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (userResponse.IsSuccessStatusCode)
                {
                    var userJsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(userJsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Chỉ lấy Staff để assign task
                    var usersList = users.Where(u => u.Role == "Staff")
                                      .Select(u => new SelectListItem
                                      {
                                          Value = u.Email,
                                          Text = $"{u.UserName} ({u.Email})"
                                      });

                    UserList = new SelectList(usersList, "Value", "Text");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                await LoadUserList();
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
                await LoadUserList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating task: {ex.Message}";
                await LoadUserList();
                return Page();
            }
        }
    }
}