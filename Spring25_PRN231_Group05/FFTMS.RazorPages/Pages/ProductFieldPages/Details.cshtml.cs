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
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ProductField ProductField { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
         
            if (id == null)
            {

                return NotFound();
            }
            var apiUrl = $"http://localhost:5281/odata/ProductFields/get-by-{id}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //ProductField = parsedResponse.;
            

            return Page();
        }
    }
}

