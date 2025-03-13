using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.Field
{
    public class DetailsFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public FieldDTO Field { get; set; }

        public DetailsFieldModel(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
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
                        TempData["ErrorMessage"] = "Field not found.";
                        return RedirectToPage("./ListField");
                    }
                    return Page();
                }
                else
                {
                    TempData["ErrorMessage"] = "Error retrieving field details.";
                    return RedirectToPage("./ListField");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToPage("./ListField");
            }
        }
    }
} 