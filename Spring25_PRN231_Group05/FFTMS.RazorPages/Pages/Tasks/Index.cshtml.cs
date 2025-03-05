using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
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

        public async Task OnGetAsync()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error connecting to API. Please try again later.";
            }
        }
    }
}