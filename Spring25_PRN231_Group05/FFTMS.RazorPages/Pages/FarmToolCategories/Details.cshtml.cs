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
    public class DetailsModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public DetailsModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public FarmToolCategoriesResponseDTO FarmToolCategories { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var apiUrl = $"https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category?$filter=farmToolCategoriesId eq '{id.Replace("'", "''")}'";
			var response = await _httpClient.GetAsync(apiUrl);
			if (!response.IsSuccessStatusCode)
			{
				return NotFound();
			}

			var jsonResponse = await response.Content.ReadAsStringAsync();
			FarmToolCategories = JsonSerializer.Deserialize<FarmToolCategoriesResponseDTO>(jsonResponse, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return Page();
		}
	}
}
