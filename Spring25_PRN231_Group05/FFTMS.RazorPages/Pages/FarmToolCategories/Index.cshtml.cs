using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System.Net.Http;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolCategories
{
    public class IndexModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public IndexModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public IList<FarmToolCategoriesResponseDTO> FarmToolCategories { get;set; } = default!;

		public async Task OnGetAsync()
		{
			var apiUrl = "https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category";

			var response = await _httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				FarmToolCategories = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(jsonResponse, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
		}
	}
}
