using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class UpdateFarmToolModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateFarmToolModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsRequestDTO FarmTools { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch existing category details from API
            var apiUrl = $"https://localhost:7207/odata/FarmTools/get-farm-tool-by-id?FarmToolsId={id}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            FarmTools = JsonSerializer.Deserialize<FarmToolsRequestDTO>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var apiUrl = "https://localhost:7207/odata/FarmTools/update-farm-tool";

            var response = await _httpClient.PutAsJsonAsync(apiUrl, FarmTools);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error updating farm tool category.");
                return Page();
            }

            return RedirectToPage("./ListFarmTools");
        }
    }
}
