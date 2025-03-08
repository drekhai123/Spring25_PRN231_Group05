using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class CreateCategoryModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public CreateCategoryModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		[BindProperty]
		public CategoryRequestDTO Cate { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			var apiUrl = "https://localhost:7207/odata/Category/create-category";
			var jsonData = new StringContent(JsonSerializer.Serialize(Cate), System.Text.Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(apiUrl, jsonData);
			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError(string.Empty, $"Error creating Product:{errorContent}");
				return Page();
			}

			return RedirectToPage("./ListCategory");
		}
	}
}
