using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FlowerFarmTaskManagementSystem.DataAccess.Repositories
{
    public class FieldRepository : IField
    {
        private readonly FlowerFarmTaskManagementSystemDbContext _context;

        public FieldRepository(FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteFieldAsync(Guid id)
        {
            var filed = await _context.Fields.FirstOrDefaultAsync(c => c.FieldId == id);
            if (filed == null) return false;

            _context.Fields.Remove(filed);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Field>> GetAllFieldAsync()
        {
            return await _context.Fields.ToListAsync();
        }

        public async Task<Field> GetFieldByIdAsync(Guid id)
        {
            return await _context.Fields.FirstOrDefaultAsync(m => m.FieldId == id);
        }

        public Task<Field> UpdateFieldAsync(Guid id, Field field)
        {
            throw new NotImplementedException();
        }

        Task<Field> IField.CreateFieldAsync(Field field)
        {
            throw new NotImplementedException();
        }
    }
}
