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
        public FarmToolsOfTaskRequestDTO FarmToolsOfTaskRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var apiUrl = $"https://localhost:7207/odata/FarmToolsOfTasks/get-all-farm-tools-of-task?$filter=FarmToolsOfTaskId eq '{id}'";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            FarmToolsOfTaskRequest = JsonSerializer.Deserialize<FarmToolsOfTaskRequestDTO>(responseBody);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var apiUrl = $"https://localhost:7207/odata/FarmToolsOfTasks/update-farm-tools-of-task-extend";
            var jsonContent = JsonSerializer.Serialize(FarmToolsOfTaskRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error updating data.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
