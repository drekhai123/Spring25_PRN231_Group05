using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;

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

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //_context.ProductFields.Add(ProductField);
            //await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
