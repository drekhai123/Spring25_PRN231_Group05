using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class UpdateCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty]
        public CategoryResponseDTO Cate { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var apiUrl = $"https://localhost:7207/odata/Category/get-categories-by-id?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(new { Message = $"Category with ID {id} not found." });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Cate = JsonSerializer.Deserialize<CategoryResponseDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Cate == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred while fetching data: {ex.Message}";
                return Page();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var apiUrl = $"https://localhost:7207/odata/Category/update-category?id={Cate.CategoryId}";

                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(Cate), System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Error updating Category.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./ListCategory");
        }
        private class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }
    }
}
