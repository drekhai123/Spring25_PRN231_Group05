using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class CreateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CreateProductModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductAddDTO Product { get; set; } = default!;
        public List<CategoryResponseDTO> CateList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync() 
        {
            try
            {
                // Get categories using the correct endpoint
                var response = await _httpClient.GetAsync("odata/Category/get-all-category");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    CateList = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<CategoryResponseDTO>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load categories.");
                }
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Reload categories
                return Page();
            }

            try
            {
                // Switch to OData URL for product creation
                _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:OdataUrl"]);
                var response = await _httpClient.PostAsJsonAsync("odata/Products", Product);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product created successfully.";
                    return RedirectToPage("./ListProduct");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error creating product: {errorContent}");
                await OnGetAsync(); // Reload categories
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                await OnGetAsync(); // Reload categories
                return Page();
            }
        }
    }

    // Helper class for OData response
/*    public class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }*/
}
