using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class CreateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateProductModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty]
        public ProductAddDTO Product { get; set; } = default!;
        public List<CategoryResponseDTO> CateList { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(Guid id) 
        {
            var apiUrl = "https://localhost:7207/odata/Category/get-all-category";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                CateList = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CategoryResponseDTO>();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var apiUrl = "https://localhost:7207/odata/Product/create-product";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating Product.");
                return Page();
            }
            return RedirectToPage("./ListProduct");
        }
    }
}
