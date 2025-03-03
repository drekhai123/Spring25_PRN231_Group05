using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class FarmToolDetailsModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public FarmToolDetailsModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		[BindProperty]
		public FarmToolsResponseDTO FarmTool { get; set; }

		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var apiUrl = $"https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category?$filter=FarmToolsId eq '{id}'";
			var response = await _httpClient.GetAsync(apiUrl);
			if (!response.IsSuccessStatusCode)
			{
				return NotFound();
			}

			var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(jsonResponse, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
            FarmTool = parsedResponse?.FirstOrDefault();

            return Page();
		}
	}
}
