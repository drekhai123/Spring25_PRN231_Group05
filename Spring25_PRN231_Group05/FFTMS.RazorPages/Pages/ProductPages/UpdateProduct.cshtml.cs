using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductPages
{
    public class UpdateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateProductModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty]
        public ProductUpdateDTO Product { get; set; } = default!;
		public List<CategoryResponseDTO> Categories { get; set; } = new();
		public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
				var apiUrl = $"https://localhost:7207/odata/Product/by-id?id={id}";
				var response = await _httpClient.GetAsync(apiUrl);

				if (!response.IsSuccessStatusCode)
				{
					return NotFound(new { Message = $"Product with ID {id} not found." });
				}

				var jsonResponse = await response.Content.ReadAsStringAsync();
				Product = JsonSerializer.Deserialize<ProductUpdateDTO>(jsonResponse, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (Product == null)
				{
					return NotFound();
				}

				// Fetch categories
				var categoryApiUrl = "https://localhost:7207/odata/Category/get-all-category";
				var categoryResponse = await _httpClient.GetAsync(categoryApiUrl);

				if (categoryResponse.IsSuccessStatusCode)
				{
					var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
					Categories = JsonSerializer.Deserialize<List<CategoryResponseDTO>>(categoryJson, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}) ?? new List<CategoryResponseDTO>();
				}

				ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName", Product.CategoryId);
			}
			catch (Exception ex)
			{
				ViewData["ErrorMessage"] = $"An error occurred while fetching data: {ex.Message}";
				return Page();
			}

			return Page();
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			try
			{
				var apiUrl = $"https://localhost:7207/odata/Product/update-product?id={Product.ProductId}";

				var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
				{
					Content = new StringContent(JsonSerializer.Serialize(Product), System.Text.Encoding.UTF8, "application/json")
				};

				var response = await _httpClient.SendAsync(request);

				if (!response.IsSuccessStatusCode)
				{
					ModelState.AddModelError(string.Empty, "Error updating Product.");
					return Page();
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}

			return RedirectToPage("./ListProduct");
		}
		private class ODataResponse<T>
		{
			public List<T>? Value { get; set; }
		}
	}
}
