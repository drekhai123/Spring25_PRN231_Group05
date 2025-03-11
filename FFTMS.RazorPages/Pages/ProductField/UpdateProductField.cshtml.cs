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
        private readonly IConfiguration _configuration;

        public UpdateProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; }
        public SelectList Products { get; set; }
        public SelectList Fields { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDTO>>("odata/Product/get-all-product");
                var fields = await _httpClient.GetFromJsonAsync<List<FieldDTO>>("odata/Field/get-all-field");
                var productField = await _httpClient.GetFromJsonAsync<ProductFieldDTO>($"odata/ProductField/by-id?id={id}");

                if (products != null && fields != null && productField != null)
                {
                    Products = new SelectList(products, "ProductId", "ProductName");
                    Fields = new SelectList(fields, "FieldId", "FieldName");
                    ProductField = productField;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading data: {ex.Message}";
            }

            return RedirectToPage("./ListProductField");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            try
            {
                ProductField.UpdatedDate = DateTime.Now;
                var response = await _httpClient.PutAsJsonAsync($"odata/ProductField/update-product-field?id={ProductField.ProductFieldId}", ProductField);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product Field updated successfully.";
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error updating Product Field: {error}");
                    await LoadSelectLists();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating Product Field: {ex.Message}");
                await LoadSelectLists();
                return Page();
            }
        }

        private async Task LoadSelectLists()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDTO>>("odata/Product/get-all-product");
                var fields = await _httpClient.GetFromJsonAsync<List<FieldDTO>>("odata/Field/get-all-field");

                if (products != null && fields != null)
                {
                    Products = new SelectList(products, "ProductId", "ProductName");
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