using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using FlowerFarmTaskManagementSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            return _mapper.Map<CategoryResponseDTO>(category);
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryRequestDTO categoryRequest)
        {
            var category = _mapper.Map<Category>(categoryRequest);
            category.CreateDate = DateTime.UtcNow;
            category.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDTO>(category);
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(Guid id, CategoryRequestDTO categoryRequest)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            _mapper.Map(categoryRequest, category);
            category.UpdateDate = DateTime.UtcNow;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDTO>(category);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
