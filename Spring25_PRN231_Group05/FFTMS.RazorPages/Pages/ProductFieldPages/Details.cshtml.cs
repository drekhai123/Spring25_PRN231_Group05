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
            var apiUrl = "https://localhost:7207/odata/get-all-productField";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<List<ProductField>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ProductField = parsedResponse?.FirstOrDefault();

            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    var apiUrl = "https://localhost:7207/odata/get-all-productField";

        //    var response = await _httpClient.PutAsJsonAsync(apiUrl, ProductField);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        ModelState.AddModelError(string.Empty, "Error  productField.");
        //        return Page();
        //    }

        //    return RedirectToPage("./Index");
        //}
    }
}

