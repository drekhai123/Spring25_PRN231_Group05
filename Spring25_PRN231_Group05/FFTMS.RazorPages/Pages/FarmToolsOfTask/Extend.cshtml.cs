using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System.Text;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolsOfTask
{
    public class ExtendModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public ExtendModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsOfTaskExtendRequestDTO FarmToolsOfTaskRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var apiUrl = $"https://localhost:7207/api/FarmToolsOfTasks/get-all-farm-tools-of-task?$filter=FarmToolsOfTaskId eq '{id}'";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<List<FarmToolsOfTaskExtendRequestDTO>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            FarmToolsOfTaskRequest = parsedResponse?.FirstOrDefault();
            if (FarmToolsOfTaskRequest == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (FarmToolsOfTaskRequest == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid request data.");
                    return Page();
                }

                var apiUrl = "https://localhost:7207/api/FarmToolsOfTasks/update-farm-tools-of-task-extend";
                var jsonContent = JsonSerializer.Serialize(FarmToolsOfTaskRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error updating data: {errorMessage}");
                    return Page();
                }

                TempData["SuccessMessage"] = "Farm tool task updated successfully.";
                return RedirectToPage("./UserTasks/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

    }
}
