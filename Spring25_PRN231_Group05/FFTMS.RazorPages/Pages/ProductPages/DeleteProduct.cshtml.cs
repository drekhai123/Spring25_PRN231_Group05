using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class DeleteProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DeleteProductModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductDTO Product { get; set; }
        
        public bool IsProductInUse { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Invalid product ID.";
                    return RedirectToPage("./ListProduct");
                }

                // Get product details
                var response = await _httpClient.GetAsync($"odata/Product/by-id?id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to retrieve the product.";
                    return RedirectToPage("./ListProduct");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<ProductDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage("./ListProduct");
                }

                // Check if product is in use
                var checkResponse = await _httpClient.GetAsync($"odata/Product/check-product-in-use?id={id}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var checkJsonResponse = await checkResponse.Content.ReadAsStringAsync();
                    IsProductInUse = JsonSerializer.Deserialize<bool>(checkJsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (IsProductInUse)
                    {
                        TempData["ErrorMessage"] = "This product cannot be deleted because it is being used in one or more product fields.";
                        return RedirectToPage("./ListProduct");
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToPage("./ListProduct");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Check again if product is in use before deletion
                var checkResponse = await _httpClient.GetAsync($"odata/Product/check-product-in-use?id={Product.ProductId}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var checkJsonResponse = await checkResponse.Content.ReadAsStringAsync();
                    IsProductInUse = JsonSerializer.Deserialize<bool>(checkJsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (IsProductInUse)
                    {
                        TempData["ErrorMessage"] = "This product cannot be deleted because it is being used in one or more product fields.";
                        return RedirectToPage("./ListProduct");
                    }
                }

                var response = await _httpClient.DeleteAsync($"odata/Product/delete-product?id={Product.ProductId}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                    return RedirectToPage("./ListProduct");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error deleting product: {error}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
} 