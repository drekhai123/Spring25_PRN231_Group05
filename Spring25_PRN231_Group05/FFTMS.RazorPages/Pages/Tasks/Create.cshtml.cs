using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Security.Claims;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskRequestDTO Task { get; set; } = new TaskRequestDTO();
        public SelectList UserList { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
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

                await LoadUserList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
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

        public async Task<IActionResult> OnPostAsync()
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

                // Lấy userId từ token và gán vào AssignedBy
                var userId = JwtHelper.GetUserIdFromToken(token);
                Task.AssignedBy = userId;

                // Set default values nếu cần
                Task.Status = true; // Active by default

                // Gửi request tạo task mới
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7207/odata/Task", Task);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = $"Error creating task: {responseContent}";
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