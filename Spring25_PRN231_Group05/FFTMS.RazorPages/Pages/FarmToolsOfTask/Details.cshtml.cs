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
using System.Net.Http;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolsOfTask
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public FarmToolsOfTaskResponseDTO FarmToolsOfTask { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
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
            var parsedResponse = JsonSerializer.Deserialize<List<FarmToolsOfTaskResponseDTO>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            FarmToolsOfTask = parsedResponse?.FirstOrDefault();
            if (FarmToolsOfTask == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
