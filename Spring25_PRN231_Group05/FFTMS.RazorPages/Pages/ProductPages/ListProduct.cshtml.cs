using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class ListProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ListProductModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:OdataUrl"]);
        }

        public List<ProductDTO> ListProduct { get; set; } = new List<ProductDTO>();
        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/Products");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<ProductDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ListProduct = odataResponse?.Value ?? new List<ProductDTO>();
                }
                else
                {
                    ErrorMessage = "Failed to load products.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading products: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                // First check if the product is in use
                var checkResponse = await _httpClient.GetAsync($"odata/Products/CheckProductInUse(id={id})");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var isInUse = await checkResponse.Content.ReadFromJsonAsync<bool>();
                    if (isInUse)
                    {
                        ErrorMessage = "This product cannot be deleted because it is being used in one or more product fields.";
                        return RedirectToPage();
                    }
                }

                // If not in use, proceed with deletion
                var response = await _httpClient.DeleteAsync($"odata/Products({id})");
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Product deleted successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to delete product: {error}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return RedirectToPage();
        }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; } = new List<T>();
    }
}
