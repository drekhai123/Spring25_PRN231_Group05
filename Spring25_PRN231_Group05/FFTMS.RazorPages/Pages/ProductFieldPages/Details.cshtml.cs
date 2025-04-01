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
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var productfield = await _context.ProductFields.FirstOrDefaultAsync(m => m.ProductFieldId == id);
            //if (productfield == null)
            //{
            //    return NotFound();
            //}
            //else
            //{
            //    ProductField = productfield;
            //}
            if (id == null)
            {
                return NotFound();
            }
            var apiUrl = $"http://localhost:5281/odata/ProductFields/get-by-id?id={id}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            // Since this is an OData API, the response might be wrapped in a "value" property
            // First, parse the JSON to check its structure
            using var jsonDoc = JsonDocument.Parse(jsonResponse);
            var root = jsonDoc.RootElement;

            ProductField parsedResponse;

            // Check if the response has a "value" property (common in OData responses)
            if (root.TryGetProperty("value", out var valueElement))
            {
                // Deserialize the "value" property into a ProductField object
                parsedResponse = JsonSerializer.Deserialize<ProductField>(valueElement.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                // If there's no "value" property, deserialize the entire response
                parsedResponse = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (parsedResponse == null)
            {
                return NotFound();
            }

            // Assign the deserialized response to the ProductField property
            ProductField = parsedResponse;
            return Page();
        }


    }
}