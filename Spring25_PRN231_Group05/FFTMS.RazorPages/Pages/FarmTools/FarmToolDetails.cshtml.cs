using Azure.Core;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
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
        public FarmToolCategoriesResponseDTO? Category { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=FarmToolsId eq '{id}'";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                var response = await _httpClient.SendAsync(request);

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
                if (FarmTool == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(FarmTool.FarmToolCategoriesId))
                {
                    var categoryApiUrl = $"https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category?$filter=FarmToolCategoriesId eq '{FarmTool.FarmToolCategoriesId}'";
                    var categoryRequest = new HttpRequestMessage(HttpMethod.Get, categoryApiUrl);
                    var categoryResponse = await _httpClient.SendAsync(categoryRequest);

                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                        var categoryParsed = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(categoryJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        Category = categoryParsed?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }

            return Page();
        }

        private class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }
    }
}
