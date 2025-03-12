using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class DeleteProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; } = new();

        public DeleteProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"odata/ProductField/by-id?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    ProductField = await response.Content.ReadFromJsonAsync<ProductFieldDTO>();
                    if (ProductField == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return RedirectToPage("./ListProductField");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"odata/ProductField/delete-product-field?id={ProductField.ProductFieldId}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete product field. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting product field: {ex.Message}");
                return Page();
            }
        }
    }
} 