using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PhamThaiDuy.NET1717.Spring25FE.Pages.FarmTools
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsResponseDTO? FarmTool { get; set; }
        public FarmToolCategoriesResponseDTO? Category { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Invalid FarmToolsId.");
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

        public async Task<IActionResult> OnPostAsync()
        {

            if (FarmTool == null || string.IsNullOrEmpty(FarmTool.FarmToolsId))
            {
                ModelState.AddModelError(string.Empty, "Invalid farm tool data.");
                return Page();
            }

            try
            {
                var apiUrl = $"https://localhost:7207/odata/FarmTools/update-farm-tools-status?FarmToolsId={FarmTool.FarmToolsId}";

                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(FarmTool), Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error updating farm tool: {errorContent}");
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
