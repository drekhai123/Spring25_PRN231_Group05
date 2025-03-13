using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.Field
{
    public class ListFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public List<FieldDTO> Fields { get; set; } = new();

        public ListFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/Field/get-all-field");
                if (response.IsSuccessStatusCode)
                {
                    Fields = await response.Content.ReadFromJsonAsync<List<FieldDTO>>() ?? new List<FieldDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error fetching fields: {ex.Message}");
                Fields = new List<FieldDTO>();
            }
        }
    }
} 