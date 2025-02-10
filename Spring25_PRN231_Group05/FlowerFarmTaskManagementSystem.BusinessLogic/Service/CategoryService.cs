using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategory _categoryRepo;

        public CategoryService(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }
            return category;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            // Business logic before creating (e.g., check for duplicates)
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("CategoryName is required.");
            }

            return await _categoryRepo.CreateCategoryAsync(category);
        }

        public async Task<Category> UpdateCategoryAsync(Guid id, Category category)
        {
            var existingCategory = await _categoryRepo.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }

            return await _categoryRepo.UpdateCategoryAsync(id, category);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }

            return await _categoryRepo.DeleteCategoryAsync(id);
        }
    }
}
