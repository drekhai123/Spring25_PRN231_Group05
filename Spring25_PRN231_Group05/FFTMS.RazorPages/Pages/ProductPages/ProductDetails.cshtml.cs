using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
        public CategoryResponseDTO Category { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Invalid product ID.";
                    return RedirectToPage("./ListProduct");
                }

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

                if (Product.CategoryId != Guid.Empty)
                {
                    var categoryApiUrl = $"https://localhost:7207/odata/Category/by-id?id={Product.CategoryId}";
                    var categoryResponse = await _httpClient.GetAsync(categoryApiUrl);
                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                        Category = JsonSerializer.Deserialize<CategoryResponseDTO>(categoryJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
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
    }
}
