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

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; } = new();
        public SelectList Products { get; set; }
        public SelectList Fields { get; set; }

        public UpdateProductFieldModel(HttpClient httpClient, IConfiguration configuration)
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

                    await LoadSelectLists();
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
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"odata/ProductField/update-product-field?id={ProductField.ProductFieldId}", ProductField);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update product field. Please try again.");
                    await LoadSelectLists();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating product field: {ex.Message}");
                await LoadSelectLists();
                return Page();
            }
        }

        private async Task LoadSelectLists()
        {
            try
            {
                var productsResponse = await _httpClient.GetAsync("odata/Product/get-all-product");
                var fieldsResponse = await _httpClient.GetAsync("odata/Field/get-all-field");

                if (productsResponse.IsSuccessStatusCode)
                {
                    var products = await productsResponse.Content.ReadFromJsonAsync<List<ProductDTO>>();
                    Products = new SelectList(products, "ProductId", "ProductName");
                }

                if (fieldsResponse.IsSuccessStatusCode)
                {
                    var fields = await fieldsResponse.Content.ReadFromJsonAsync<List<FieldDTO>>();
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