using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class UpdateProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; }

        public SelectList Products { get; set; }
        public SelectList Fields { get; set; }

        public UpdateProductFieldModel(IConfiguration configuration)
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

                    await LoadDropdownData();
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
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"odata/ProductField/update-product-field?id={ProductField.ProductFieldId}", ProductField);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product Field updated successfully.";
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error updating product field: {error}");
                    await LoadDropdownData();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                await LoadDropdownData();
                return Page();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                var productsResponse = await _httpClient.GetAsync("odata/Product/get-all-product");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var products = await productsResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
                    Products = new SelectList(products, "ProductId", "ProductName");
                }

                var fieldsResponse = await _httpClient.GetAsync("odata/Field/get-all-field");
                if (fieldsResponse.IsSuccessStatusCode)
                {
                    var fields = await fieldsResponse.Content.ReadFromJsonAsync<IEnumerable<FieldDTO>>();
                    Fields = new SelectList(fields, "FieldId", "FieldName");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading dropdown data: {ex.Message}");
            }
        }
    }
}