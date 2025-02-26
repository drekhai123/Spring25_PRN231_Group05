using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;

namespace FFTMS.RazorPages.Pages.FarmToolCategories
{
    public class DeleteModel : PageModel
    {
        private readonly FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext _context;

        public DeleteModel(FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmToolCategories FarmToolCategories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmtoolcategories = await _context.FarmToolCategories.FirstOrDefaultAsync(m => m.FarmToolCategoriesId == id);

            if (farmtoolcategories == null)
            {
                return NotFound();
            }
            else
            {
                FarmToolCategories = farmtoolcategories;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmtoolcategories = await _context.FarmToolCategories.FindAsync(id);
            if (farmtoolcategories != null)
            {
                FarmToolCategories = farmtoolcategories;
                _context.FarmToolCategories.Remove(FarmToolCategories);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
