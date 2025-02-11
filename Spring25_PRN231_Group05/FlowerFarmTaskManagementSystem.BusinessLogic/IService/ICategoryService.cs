using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CategoryRequestDTO categoryRequest);
        Task<CategoryResponseDTO> UpdateCategoryAsync(Guid id, CategoryRequestDTO categoryRequest);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
