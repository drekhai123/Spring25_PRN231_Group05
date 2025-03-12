using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class ListProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IEnumerable<ProductFieldDTO> ProductFields { get; set; }

        public ListProductFieldModel(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
        }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/ProductField/get-all-product-field");
                if (response.IsSuccessStatusCode)
                {
                    ProductFields = await response.Content.ReadFromJsonAsync<IEnumerable<ProductFieldDTO>>();
                }
                else
                {
                    TempData["ErrorMessage"] = "Error retrieving product fields.";
                    ProductFields = Enumerable.Empty<ProductFieldDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                ProductFields = Enumerable.Empty<ProductFieldDTO>();
            }
        }
    }
} 