using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class UpdateProductivityModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateProductivityModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5281/");
        }

        [BindProperty]
        public ProductFieldResponse ProductField { get; set; }

        public string ErrorMessage { get; set; }
        [BindProperty]
        public string Unit { get; set; } = string.Empty;
        [BindProperty]
        public double Quantity { get; set; } = 0;
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"odata/ProductField/{id}?$expand=Product,Field";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        ErrorMessage = "API returned an empty response.";
                        return Page();
                    }

                    ProductFieldResponse productField;
                    using var jsonDoc = JsonDocument.Parse(jsonResponse);
                    var root = jsonDoc.RootElement;

                    if (root.ValueKind == JsonValueKind.Array || !root.TryGetProperty("value", out var valueElement))
                    {
                        productField = JsonSerializer.Deserialize<ProductFieldResponse>(jsonResponse, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    else
                    {
                        productField = JsonSerializer.Deserialize<ProductFieldResponse>(valueElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }

                    if (productField == null)
                    {
                        ErrorMessage = "No product field data returned.";
                        return Page();
                    }

                    ProductField = productField;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Error fetching ProductField: {response.StatusCode} - {errorResponse}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error fetching ProductField: {ex.Message}";
                return Page();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var id = ProductField.ProductFieldId.ToString();

                if (string.IsNullOrEmpty(id))
                {
                    ModelState.AddModelError(string.Empty, "Invalid request data: ProductFieldId is null or empty.");
                    return Page();
                }

                if (!string.IsNullOrWhiteSpace(Unit) && (Quantity <= 0))
                {
                    TempData["ErrorMessage"] = "Quantity must be greater than 0 when Unit is provided.";
                    return RedirectToPage("/ProductFieldPages/Index");
                }

                if (Quantity > 0 && string.IsNullOrWhiteSpace(Unit))
                {
                    TempData["ErrorMessage"] = "Unit is required when Quantity is provided.";
                    return RedirectToPage("/ProductFieldPages/Index");
                }

                var apiUrl = $"http://localhost:5281/odata/ProductField/update-product-field-productivity?id={id}&Productivity={Quantity}&ProductivityUnit={Unit}";

                var response = await _httpClient.PutAsync(apiUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToPage("/ProductFieldPages/Index");
                }

                TempData["SuccessMessage"] = "Productivity updated successfully.";
                return RedirectToPage("/ProductFieldPages/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/ProductFieldPages/Index");
            }
        }
    }
}
