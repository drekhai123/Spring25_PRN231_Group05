using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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

                    Task = new TaskRequestDTO
                    {
                        JobTitle = taskResponse.JobTitle,
                        Description = taskResponse.Description,
                        AssignedTo = taskResponse.AssignedTo,
                        AssignedBy = userId,
                        StartDate = taskResponse.StartDate,
                        EndDate = taskResponse.EndDate,
                        Status = taskResponse.Status,
                        ImageUrl = taskResponse.ImageUrl
                    };

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

                ErrorMessage = "Error updating task";
                await LoadUserList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await LoadUserList();
                return Page();
            }
        }
    }
}