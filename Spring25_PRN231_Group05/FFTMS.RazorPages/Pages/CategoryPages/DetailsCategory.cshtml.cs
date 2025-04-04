using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class DetailsCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7207/");
        }

        public CategoryResponseDTO Category { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try 
            {
                var apiUrl = $"odata/Category/get-categories-by-id?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Category = JsonSerializer.Deserialize<CategoryResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (Category == null)
                    {
                        return NotFound();
                    }

                    return Page();
                }
                else 
                {
                    ErrorMessage = "Failed to retrieve category details. Please try again later.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
} 