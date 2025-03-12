using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.Field
{
    public class UpdateFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public FieldDTO Field { get; set; } = new();

        public UpdateFieldModel(HttpClient httpClient, IConfiguration configuration)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"odata/Field/update-field?id={Field.FieldId}", Field);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update field. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating field: {ex.Message}");
                return Page();
            }
        }
    }
} 