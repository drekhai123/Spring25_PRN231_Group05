using Azure.Core;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmTools
{
    public class UpdateFarmToolModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateFarmToolModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsRequestDTO FarmTools { get; set; } = default!;
        public List<FarmToolCategoriesResponseDTO> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=FarmToolsId eq '{id}' and Status eq true";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonSerializer.Deserialize<List<FarmToolsRequestDTO>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                FarmTools = parsedResponse?.FirstOrDefault();
                if (FarmTools == null)
                {
                    return NotFound();
                }

                var categoryApiUrl = "https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category";
                var categoryRequest = new HttpRequestMessage(HttpMethod.Get, categoryApiUrl);
                var categoryResponse = await _httpClient.SendAsync(categoryRequest);

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                    var categoryParsed = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(categoryJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Categories = categoryParsed ?? new List<FarmToolCategoriesResponseDTO>();
                }

                ViewData["FarmToolCategoriesId"] = new SelectList(Categories, "FarmToolCategoriesId", "FarmToolCategoriesName", FarmTools.FarmToolCategoriesId);
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
                var apiUrl = $"https://localhost:7207/odata/FarmTools/update-farm-tools?FarmToolsId={FarmTools.FarmToolsId}";

                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(FarmTools), System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Error updating farm tool.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./ListFarmTools");
        }

        private class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }
    }
}

