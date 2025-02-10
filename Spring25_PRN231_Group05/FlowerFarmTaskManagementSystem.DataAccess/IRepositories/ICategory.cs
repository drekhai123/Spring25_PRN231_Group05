using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.DataAccess.IRepositories
{
    public interface ICategory
    {
        IEnumerable<Category> GetAllCategories();
        Task<Category> GetCategoryById(Guid id);
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Guid id, Category category);
        Task<Category> DeleteCategory(Guid id);
    }
}
