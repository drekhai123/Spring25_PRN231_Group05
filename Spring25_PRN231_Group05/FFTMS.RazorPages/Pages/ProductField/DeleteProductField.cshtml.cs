using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class DeleteProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; }

        public DeleteProductFieldModel(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
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
                        TempData["ErrorMessage"] = "Product Field not found.";
                        return RedirectToPage("./ListProductField");
                    }
                    return Page();
                }
                else
                {
                    TempData["ErrorMessage"] = "Error retrieving product field.";
                    return RedirectToPage("./ListProductField");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
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
                    TempData["SuccessMessage"] = "Product Field deleted successfully.";
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error deleting product field: {error}";
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