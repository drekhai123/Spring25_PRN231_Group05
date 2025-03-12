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

        public CreateProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductFieldDTO ProductField { get; set; }
        public SelectList Products { get; set; }
        public SelectList Fields { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDTO>>("odata/Product/get-all-product");
                var fields = await _httpClient.GetFromJsonAsync<List<FieldDTO>>("odata/Field/get-all-field");

                if (products != null && fields != null)
                {
                    Products = new SelectList(products, "ProductId", "ProductName");
                    Fields = new SelectList(fields, "FieldId", "FieldName");
                    ProductField = new ProductFieldDTO
                    {
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(30),
                        Status = true,
                        CreatedDate = DateTime.Now,
                        Product = new ProductDTO(),
                        Field = new FieldDTO()
                    };
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
                await OnGetAsync(); // Reload the dropdowns
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("odata/ProductField/create-product-field", ProductField);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product Field created successfully.";
                    return RedirectToPage("./ListProductField");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error creating Product Field: {error}");
                    await OnGetAsync(); // Reload the dropdowns
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating Product Field: {ex.Message}");
                await OnGetAsync(); // Reload the dropdowns
                return Page();
            }
        }
    }
}