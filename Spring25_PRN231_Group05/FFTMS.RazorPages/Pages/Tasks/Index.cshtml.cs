using FFTMS.RazorPages.Helpers;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IList<TaskResponseDTO> Tasks { get; set; } = new List<TaskResponseDTO>();
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

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

                // Xây dựng query OData
                var query = "https://localhost:7207/odata/Task";
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query += $"?$filter=contains(tolower(JobTitle),tolower('{SearchTerm}')) " +
                            $"or contains(tolower(Description),tolower('{SearchTerm}'))";
                }

                // Lấy danh sách tasks
                var taskResponse = await _httpClient.GetAsync(query);
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");

                if (taskResponse.IsSuccessStatusCode && userResponse.IsSuccessStatusCode)
                {
                    var jsonTaskResponse = await taskResponse.Content.ReadAsStringAsync();
                    var jsonUserResponse = await userResponse.Content.ReadAsStringAsync();

                    var tasks = JsonSerializer.Deserialize<List<TaskResponseDTO>>(jsonTaskResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<TaskResponseDTO>();

                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonUserResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UserResponseDTO>();

                    // Map username vào AssignedTo
                    foreach (var task in tasks)
                    {
                        if (!string.IsNullOrEmpty(task.AssignedTo))
                        {
                            var user = users.FirstOrDefault(u => u.UserId.ToString() == task.AssignedTo);
                            if (user != null)
                            {
                                task.AssignedTo = user.UserName;
                            }
                        }
                    }

                    Tasks = tasks;
                }
                else
                {
                    ErrorMessage = $"API returned status code: {taskResponse.StatusCode}";
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error connecting to API. Please try again later.";
                return Page();
            }
        }
    }
}