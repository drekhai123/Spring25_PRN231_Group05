using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5281/"); // Set base address for consistency
        }

        [BindProperty]
        public ProductField ProductField { get; set; } = new ProductField();

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Fetch products for dropdown
                var productResponse = await _httpClient.GetAsync("odata/Products");
                if (productResponse.IsSuccessStatusCode)
                {
                    var productJson = await productResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response (Products): {productJson}");
                    var products = JsonSerializer.Deserialize<List<ProductDTO>>(productJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<ProductDTO>();
                    ViewData["ProductId"] = new SelectList(products, "ProductId", "ProductName");
                }
                else
                {
                    var errorResponse = await productResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error (Products): {productResponse.StatusCode} - {errorResponse}");
                    ErrorMessage = $"Failed to load products: {productResponse.StatusCode} - {errorResponse}";
                }

                // Fetch fields for dropdown (uncomment and adjust endpoint as needed)
                var fieldResponse = await _httpClient.GetAsync("odata/Fields"); // Adjusted endpoint assumption
                if (fieldResponse.IsSuccessStatusCode)
                {
                    var fieldJson = await fieldResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response (Fields): {fieldJson}");
                    var fields = JsonSerializer.Deserialize<List<FieldDTO>>(fieldJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<FieldDTO>();
                    ViewData["FieldId"] = new SelectList(fields, "FieldId", "FieldName"); // Changed to FieldName for display
                }
                else
                {
                    var errorResponse = await fieldResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error (Fields): {fieldResponse.StatusCode} - {errorResponse}");
                    ErrorMessage = $"Failed to load fields: {fieldResponse.StatusCode} - {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                ErrorMessage = $"Error fetching data: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("OnPostAsync: ModelState is invalid.");
                return Page();
            }

            try
            {
                var apiUrl = "odata/create-productField";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, ProductField);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OnPostAsync: ProductField created successfully. Redirecting to Index...");
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorResponse}");
                    ErrorMessage = $"API Error: {response.StatusCode} - {errorResponse}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product field: {ex.Message}");
                ErrorMessage = $"Error creating product field: {ex.Message}";
                return Page();
            }
        }
    }
}