using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class CreateCategoryModel : PageModel
    {
		private readonly HttpClient _httpClient;

		public CreateCategoryModel(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri("https://localhost:7207/");
		}
		[BindProperty]
		public CreateCategoryInputModel Input { get; set; } = new();

		public string? ErrorMessage { get; set; }
		public string? SuccessMessage { get; set; }

		public class CreateCategoryInputModel
		{
			[Required(ErrorMessage = "Category name is required")]
			[StringLength(30, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 30 characters")]
			[Display(Name = "Category Name")]
			public string CategoryName { get; set; } = string.Empty;

			[Required(ErrorMessage = "Description is required")]
			[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
			public string Description { get; set; } = string.Empty;

			[Required(ErrorMessage = "Image is required")]
			[Display(Name = "Category Image URL")]
			public string CategoryImageUrl { get; set; } = string.Empty;

			[Required(ErrorMessage = "Status is required")]
			public bool Status { get; set; } = true;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				var createData = new
				{
					categoryName = Input.CategoryName,
					description = Input.Description,
					categoryImageUrl = Input.CategoryImageUrl,
					status = Input.Status
				};

				var response = await _httpClient.PostAsJsonAsync("odata/Category/create-category", createData);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToPage("./ListCategory");
				}

				var errorContent = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError(string.Empty, $"Error creating category: {errorContent}");
				return Page();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}
	}
}
