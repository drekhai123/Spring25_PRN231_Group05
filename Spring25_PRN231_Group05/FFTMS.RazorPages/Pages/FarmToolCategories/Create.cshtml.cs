using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.FarmToolCategories
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolCategoriesRequestDTO FarmToolCategories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var apiUrl = "https://localhost:7207/odata/FarmToolCategories/create-farm-tool-category";

            var response = await _httpClient.PostAsJsonAsync(apiUrl, FarmToolCategories); 

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating farm tool category.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
