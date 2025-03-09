using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class ListCategoryModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public ListCategoryModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public List<CategoryResponseDTO> ListProduct { get; set; } = new List<CategoryResponseDTO>();
		public async Task OnGetAsync()
		{
			var apiUrl = "https://localhost:7207/odata/Category/get-all-category";
			var response = await _httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				ListProduct = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(jsonResponse, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
		}
	}
}
