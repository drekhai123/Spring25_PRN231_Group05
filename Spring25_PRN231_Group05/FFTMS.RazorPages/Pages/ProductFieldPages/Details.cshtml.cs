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
    public class DetailsModel : PageModel
    {
        private readonly FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext _context;

        public DetailsModel(FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        public ProductField ProductField { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productfield = await _context.ProductFields.FirstOrDefaultAsync(m => m.ProductFieldId == id);
            if (productfield == null)
            {
                return NotFound();
            }
            else
            {
                ProductField = productfield;
            }
            return Page();
        }
    }
}
