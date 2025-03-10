using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class EditModel : PageModel
    {
        private readonly FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext _context;

        public EditModel(FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductField ProductField { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productfield =  await _context.ProductFields.FirstOrDefaultAsync(m => m.ProductFieldId == id);
            if (productfield == null)
            {
                return NotFound();
            }
            ProductField = productfield;
           ViewData["FieldId"] = new SelectList(_context.Fields, "FieldId", "Description");
           ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Description");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ProductField).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductFieldExists(ProductField.ProductFieldId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductFieldExists(Guid id)
        {
            return _context.ProductFields.Any(e => e.ProductFieldId == id);
        }
    }
}
