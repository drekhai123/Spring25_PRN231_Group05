using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IList<UserResponseDTO> Users { get; set; } = new List<UserResponseDTO>();
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7207/odata/User");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var allUsers = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UserResponseDTO>();

                    // Filter out users with Admin role
                    Users = allUsers.Where(u => u.Role != "Admin").ToList();
                }
                else
                {
                    ErrorMessage = $"API returned status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error connecting to API. Please try again later.";
            }
        }
    }
}