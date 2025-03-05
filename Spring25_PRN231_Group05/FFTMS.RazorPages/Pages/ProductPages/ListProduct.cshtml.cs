using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class ListProductModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public ListProductModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public List<ProductDTO> ListProduct { get; set; } = new List<ProductDTO>();
		public async Task OnGetAsync()
        {
			var apiUrl = "https://localhost:7207/odata/Product/get-all-product";
			var response = await _httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				ListProduct = JsonSerializer.Deserialize<List<ProductDTO>>(jsonResponse, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
		}
    }
}
