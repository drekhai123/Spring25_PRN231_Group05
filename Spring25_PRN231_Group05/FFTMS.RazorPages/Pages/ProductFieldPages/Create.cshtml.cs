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

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public IActionResult OnGet()
        //{
        //ViewData["FieldId"] = new SelectList(_context.Fields, "FieldId", "Description");
        //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Description");
        //    return Page();
        //}

        [BindProperty]
        public ProductField ProductField { get; set; } = default!;
        //public List<ProductFieldResponse>() {get; set} = new();
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var apiUrl = "https://localhost:7207/odata/create-productField";

            var response = await _httpClient.PostAsJsonAsync(apiUrl, ProductField);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating farm tool category.");
                return Page();
            }

            return RedirectToPage("./Index");

        }
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
    }
}

