using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class CreateFarmToolsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateFarmToolsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsRequestDTO FarmTools { get; set; } = default!;
        public List<FarmToolCategoriesResponseDTO> FarmToolCategoriesList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(String? id)
        {
            var apiUrl = "https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                FarmToolCategoriesList = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<FarmToolCategoriesResponseDTO>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var apiUrl = "https://localhost:7207/odata/FarmTools/create-farm-tool";

            var response = await _httpClient.PostAsJsonAsync(apiUrl, FarmTools);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating farm tool.");
                return Page();
            }

            return RedirectToPage("./ListFarmTools");
        }
    }
}
