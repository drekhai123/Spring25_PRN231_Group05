using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductField
{
    public class DetailsProductFieldModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductFieldDTO ProductField { get; set; } = new();

        public DetailsProductFieldModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"odata/ProductField/by-id?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    ProductField = await response.Content.ReadFromJsonAsync<ProductFieldDTO>();
                    if (ProductField == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return RedirectToPage("./ListProductField");
            }
        }
    }
} 