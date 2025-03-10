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
    public class IndexModel : PageModel
    {
        private readonly FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext _context;

        public IndexModel(FlowerFarmTaskManagementSystem.DataAccess.FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        public IList<ProductField> ProductField { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ProductField = await _context.ProductFields
                .Include(p => p.Field)
                .Include(p => p.Product).ToListAsync();
        }
    }
}
