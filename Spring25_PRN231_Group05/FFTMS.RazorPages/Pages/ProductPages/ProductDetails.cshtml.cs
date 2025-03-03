using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class ProductDetailsModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public ProductDetailsModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		[BindProperty]
		public ProductDTO Product { get; set; }
		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var apiUrl = $"https://localhost:7207/odata/Product/by-id/{id}";
			var response = await _httpClient.GetAsync(apiUrl);
			if (!response.IsSuccessStatusCode)
			{
				return NotFound();
			}

			var jsonResponse = await response.Content.ReadAsStringAsync();
			var parsedResponse = JsonSerializer.Deserialize<List<ProductDTO>>(jsonResponse, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
			Product = parsedResponse?.FirstOrDefault();

			return Page();
		}
	}
}
