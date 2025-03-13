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

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IList<ProductFieldResponse> ProductField { get;set; } = new List<ProductFieldResponse>();

        public async Task OnGetAsync()
        {
            var apiUrl = "https://localhost:7207/odata/ProductField/get-all-productField";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                ProductField = JsonSerializer.Deserialize<List<ProductFieldResponse>>(jsonResponse, 
                    new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
      
    }
}
