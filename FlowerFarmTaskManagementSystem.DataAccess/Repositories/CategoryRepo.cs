using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.DataAccess.Repositories
{
    public class CategoryRepo : ICategory
    {
        private readonly FlowerFarmTaskManagementSystemDbContext _context;

        public CategoryRepo(FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CategoryId = Guid.NewGuid();
            category.CreateDate = DateTime.Now;
            category.UpdateDate = DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Guid id, Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (existingCategory == null) return null;

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;
            existingCategory.CategoryImageUrl = category.CategoryImageUrl;
            existingCategory.UpdateDate = DateTime.Now;
            existingCategory.Status = category.Status;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
