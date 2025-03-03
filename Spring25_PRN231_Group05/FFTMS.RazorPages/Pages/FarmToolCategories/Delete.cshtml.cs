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
using System.Net.Http.Headers;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolCategories
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolCategoriesResponseDTO FarmToolCategories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"https://localhost:7207/odata/FarmToolCategories/get-all-farm-tool-category?$filter=FarmToolCategoriesId eq '{id}'";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonSerializer.Deserialize<List<FarmToolCategoriesResponseDTO>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                FarmToolCategories = parsedResponse?.FirstOrDefault();
                if (FarmToolCategories == null)
                {
                    return NotFound();
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
            try
            {
                var apiUrl = $"https://localhost:7207/odata/FarmToolCategories/update-farm-tool-category-status?FarmToolCategoriesId={FarmToolCategories.FarmToolCategoriesId}";

                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(FarmToolCategories), System.Text.Encoding.UTF8, "application/json")
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

            return RedirectToPage("./Index");
        }

        private class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }
    }
}

