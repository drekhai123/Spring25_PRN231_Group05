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

                var response = await _httpClient.GetAsync("https://localhost:7207/odata/Task");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Tasks = JsonSerializer.Deserialize<List<TaskResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<TaskResponseDTO>();
                }
                else
                {
                    ErrorMessage = $"API returned status code: {response.StatusCode}";
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