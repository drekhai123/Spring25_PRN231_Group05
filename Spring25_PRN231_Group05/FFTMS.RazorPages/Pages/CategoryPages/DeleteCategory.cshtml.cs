using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class DeleteCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7207/");
        }

        [BindProperty]
        public CategoryResponseDTO Category { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"odata/Category/get-categories-by-id?id={id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không tìm thấy danh mục.";
                    return Page();
                }

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
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"odata/Category/delete-category?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListCategory");
                }

                var error = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Lỗi khi xóa danh mục: {error}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return Page();
            }
        }
    }
}
