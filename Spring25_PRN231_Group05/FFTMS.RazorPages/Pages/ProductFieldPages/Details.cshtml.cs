using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5281/");
        }

        public ProductFieldResponse ProductField { get; set; }

        public string ErrorMessage { get; set; }

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

                    // Kiểm tra nếu response rỗng
                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        Console.WriteLine("OnGetAsync: API returned an empty response.");
                        ErrorMessage = "API trả về phản hồi trống.";
                        return Page();
                    }

                    // Phân tích JSON
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
                        return NotFound();
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
    }
}
