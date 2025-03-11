using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var productfield = await _context.ProductFields.FindAsync(id);
            //if (productfield != null)
            //{
            //    ProductField = productfield;
            //    _context.ProductFields.Remove(ProductField);
            //    await _context.SaveChangesAsync();
            //}

            return RedirectToPage("./Index");
        }
    }
}
