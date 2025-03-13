using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.Field
{
    public class CreateFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public FieldDTO Field { get; set; } = new();

        public CreateFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public void OnGet()
        {
            // Initialize any default values if needed
            Field.Status = true; // Set default status to active
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("odata/Field/create-field", Field);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListField");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create field. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating field: {ex.Message}");
                return Page();
            }
        }
    }
} 