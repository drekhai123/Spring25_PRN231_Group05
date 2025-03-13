using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class ListProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public List<ProductFieldDTO> ProductFields { get; set; } = new();

        public ListProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/ProductField/get-all-product-field");
                if (response.IsSuccessStatusCode)
                {
                    ProductFields = await response.Content.ReadFromJsonAsync<List<ProductFieldDTO>>() ?? new List<ProductFieldDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error fetching product fields: {ex.Message}");
                ProductFields = new List<ProductFieldDTO>();
            }
        }
    }
} 