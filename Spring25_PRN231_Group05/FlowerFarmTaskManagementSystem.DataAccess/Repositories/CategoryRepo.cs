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

        public async Task<Category> CreateCategory(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> UpdateCategory(Guid id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return null;

            existingCategory.CategoryName = category.CategoryName ?? existingCategory.CategoryName;
            existingCategory.Description = category.Description ?? existingCategory.Description;
            existingCategory.UpdateDate = DateTime.UtcNow;
            existingCategory.CategoryImageUrl = category.CategoryImageUrl ?? existingCategory.CategoryImageUrl;
            existingCategory.Status = category.Status;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            return existingCategory;
        }
    }
}
