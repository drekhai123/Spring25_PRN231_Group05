using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.UserTasks
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<UserTaskResponseDTO> UserTasks { get; set; } = new();
        public string ErrorMessage { get; set; }

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
                if (role != "Staff")
                {
                    return RedirectToPage("/Index");
                }

                var userId = JwtHelper.GetUserIdFromToken(token);
                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/UserTask?$filter=UserId eq {userId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    UserTasks = JsonSerializer.Deserialize<List<UserTaskResponseDTO>>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    ErrorMessage = "Failed to load tasks.";
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}