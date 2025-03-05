using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class CreateFarmToolsModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public CreateFarmToolsModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		[BindProperty]
		public FarmToolsRequestDTO FarmTools { get; set; } = new FarmToolsRequestDTO();

		public List<FarmToolCategoriesResponseDTO> FarmToolCategoriesList { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{

			try
			{
				var apiUrl = "https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category?$filter=Status eq true";
				var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

				var response = await _httpClient.SendAsync(request);
				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();
					var parsedResponse = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(jsonResponse, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					FarmToolCategoriesList = parsedResponse ?? new List<FarmToolCategoriesResponseDTO>();
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Error fetching categories: {ex.Message}");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{

			try
			{
				var apiUrl = "https://localhost:7207/odata/FarmTools/create-farm-tools";
				var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
				{
					Content = JsonContent.Create(FarmTools)
				};

				var response = await _httpClient.SendAsync(request);

				if (!response.IsSuccessStatusCode)
				{
					string errorMessage = await response.Content.ReadAsStringAsync();
					ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");
					return Page();
				}

				return RedirectToPage("./ListFarmTools");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}

		private class ODataResponse<T>
		{
			public List<T>? Value { get; set; }
		}
	}
}
