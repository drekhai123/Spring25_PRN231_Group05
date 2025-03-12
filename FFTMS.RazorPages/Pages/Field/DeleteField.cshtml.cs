using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.Field
{
    public class DeleteFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public FieldDTO Field { get; set; } = new();

        public DeleteFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"odata/Field/by-id?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    Field = await response.Content.ReadFromJsonAsync<FieldDTO>();
                    if (Field == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return RedirectToPage("./ListField");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"odata/Field/delete-field?id={Field.FieldId}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete field. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting field: {ex.Message}");
                return Page();
            }
        }
    }
} 