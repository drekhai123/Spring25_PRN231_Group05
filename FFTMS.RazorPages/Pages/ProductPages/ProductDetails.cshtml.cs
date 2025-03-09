using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProductDetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                    return NotFound();
                }

                var apiUrl = $"https://localhost:7207/odata/Product/by-id?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Product = JsonSerializer.Deserialize<ProductDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Product == null)
                {
                    return NotFound();
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
                ViewData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
} 