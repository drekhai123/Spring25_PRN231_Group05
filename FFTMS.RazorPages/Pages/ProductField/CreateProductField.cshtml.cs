using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class CreateProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; } = new();
        public SelectList Products { get; set; }
        public SelectList Fields { get; set; }

        public CreateProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectLists();
            ProductField.Status = true; // Set default status to active
            return Page();
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
                var response = await _httpClient.PostAsJsonAsync("odata/ProductField/create-product-field", ProductField);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create product field. Please try again.");
                    await LoadSelectLists();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating product field: {ex.Message}");
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