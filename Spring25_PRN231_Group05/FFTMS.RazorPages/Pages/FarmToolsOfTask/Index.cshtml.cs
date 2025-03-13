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
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolsOfTask
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IList<FarmToolsOfTaskResponseDTO> FarmToolsOfTask { get; set; } = new List<FarmToolsOfTaskResponseDTO>();

        public async Task OnGetAsync()
        {
            var apiUrl = "https://localhost:7207/odata/FarmToolsOfTasks/get-all-farm-tools-of-task?$orderby=Status desc";

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                FarmToolsOfTask = JsonSerializer.Deserialize<List<FarmToolsOfTaskResponseDTO>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                                  ?? new List<FarmToolsOfTaskResponseDTO>();
            }
            else
            {
                ModelState.AddModelError("", "Failed to load farm tools tasks.");
            }
        }
    }
}
