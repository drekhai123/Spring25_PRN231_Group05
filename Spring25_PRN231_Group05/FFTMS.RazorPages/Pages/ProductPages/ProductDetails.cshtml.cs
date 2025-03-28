using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductDetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductDTO Product { get; set; }
        public bool IsInUse { get; set; }
        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    ErrorMessage = "Invalid product ID.";
                    return RedirectToPage("./ListProduct");
                }

                var response = await _httpClient.GetAsync($"odata/Product/by-id?id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Failed to retrieve the product.";
                    return RedirectToPage("./ListProduct");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<ProductDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Product == null)
                {
                    ErrorMessage = "Product not found.";
                    return RedirectToPage("./ListProduct");
                }

                // Check if product is in use
                var checkResponse = await _httpClient.GetAsync($"odata/Product/check-product-in-use?id={id}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    IsInUse = await checkResponse.Content.ReadFromJsonAsync<bool>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return RedirectToPage("./ListProduct");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                if (await CheckProductInUse(id))
                {
                    ErrorMessage = "This product cannot be deleted because it is being used in one or more product fields.";
                    return RedirectToPage(new { id });
                }

                var response = await _httpClient.DeleteAsync($"odata/Product/delete-product?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Product deleted successfully.";
                    return RedirectToPage("./ListProduct");
                }
                else
                {
                    ErrorMessage = "Failed to delete the product.";
                    return RedirectToPage(new { id });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while deleting: {ex.Message}";
                return RedirectToPage(new { id });
            }
        }

        private async Task<bool> CheckProductInUse(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"odata/Product/check-product-in-use?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
