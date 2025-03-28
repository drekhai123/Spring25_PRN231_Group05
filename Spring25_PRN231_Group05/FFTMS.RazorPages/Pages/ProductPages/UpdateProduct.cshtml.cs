using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class UpdateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UpdateProductModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductDTO Product { get; set; }
        public List<CategoryResponseDTO> CateList { get; set; } = new();
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

                // Get product details
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

                // Get categories
                var categoryResponse = await _httpClient.GetAsync("odata/Category/get-all-category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                    CateList = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(categoryJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<CategoryResponseDTO>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return RedirectToPage("./ListProduct");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadCategories();
                    return Page();
                }

                var response = await _httpClient.PutAsJsonAsync($"odata/Product/update-product?id={Product.ProductId}", Product);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Product updated successfully.";
                    return RedirectToPage("./ProductDetails", new { id = Product.ProductId });
                }
                else
                {
                    ErrorMessage = "Failed to update the product.";
                    await LoadCategories();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                await LoadCategories();
                return Page();
            }
        }

        private async Task LoadCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync("odata/Category/get-all-category");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    CateList = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<CategoryResponseDTO>();
                }
            }
            catch
            {
                CateList = new List<CategoryResponseDTO>();
            }
        }
    }
}
