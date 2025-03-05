using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class ListFarmToolsModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public ListFarmToolsModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public List<FarmToolsResponseDTO> FarmTools { get; set; } = new List<FarmToolsResponseDTO>();

		public async Task OnGetAsync()
		{
			var apiUrl = "https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=Status eq true";

			var response = await _httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				FarmTools = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(jsonResponse, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
		}
	}
}
