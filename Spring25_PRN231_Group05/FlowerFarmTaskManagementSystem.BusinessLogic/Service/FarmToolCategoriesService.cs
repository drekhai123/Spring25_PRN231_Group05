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
	public class FarmToolCategoriesService : IFarmToolCategoriesService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public FarmToolCategoriesService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<FarmToolCategoriesResponseDTO> CreateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest)
		{
			var farmToolCategories = new FarmToolCategories();
			farmToolCategories.FarmToolCategoriesId = Guid.NewGuid();
			farmToolCategories.FarmToolCategoriesName = farmToolCategoriesRequest.FarmToolCategoriesName;
			farmToolCategories.FarmToolCategoriesDescription = farmToolCategoriesRequest.FarmToolCategoriesDescription;
			farmToolCategories.CreateDate = DateTime.UtcNow;
			farmToolCategories.UpdateDate = DateTime.UtcNow;
			farmToolCategories.Status = true;
			await _unitOfWork.FarmToolCategoriesRepository.AddAsync(farmToolCategories);
			await _unitOfWork.SaveChangesAsync();
			var farmToolCategoriesMap = _mapper.Map<FarmToolCategoriesResponseDTO>(farmToolCategories);
			//farmToolCategoriesMap.FarmToolCategoriesId = farmToolCategories.FarmToolCategoriesId.ToString();
			return farmToolCategoriesMap;
		}
		public async Task<bool> DeleteFarmToolCategoriesAsync(string FarmToolCategoriesId)
		{
			var FarmToolCategoryId = Guid.Parse(FarmToolCategoriesId);
			var farmToolCategories = await _unitOfWork.FarmToolCategoriesRepository.GetByIdAsync(FarmToolCategoryId);
			if (farmToolCategories == null)
			{
				throw new KeyNotFoundException("Farm Tool Categories not found.");
			}
			_unitOfWork.FarmToolCategoriesRepository.Delete(farmToolCategories);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}
		public async Task<IEnumerable<FarmToolCategoriesResponseDTO>> GetAllFarmToolCategoriesAsync()
		{
			var farmToolCategories = await _unitOfWork.FarmToolCategoriesRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<FarmToolCategoriesResponseDTO>>(farmToolCategories);
		}
		public async Task<FarmToolCategoriesResponseDTO> GetFarmToolCategoryByIdAsync(string FarmToolCategoriesId)
		{
			var FarmToolCategoryId = Guid.Parse(FarmToolCategoriesId);
			var farmToolCategories = await _unitOfWork.FarmToolCategoriesRepository.GetByIdAsync(FarmToolCategoryId);
			if (farmToolCategories == null)
			{
				throw new KeyNotFoundException("Farm Tool Categories not found.");
			}
			var farmToolCategoriesMap = _mapper.Map<FarmToolCategoriesResponseDTO>(farmToolCategories);
			return farmToolCategoriesMap;
		}
		public async Task<FarmToolCategoriesResponseDTO> UpdateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest)
		{
			var FarmToolCategoryId = Guid.Parse(farmToolCategoriesRequest.FarmToolCategoriesId);
			var farmToolCategories = await _unitOfWork.FarmToolCategoriesRepository.GetByIdAsync(FarmToolCategoryId);
			if (farmToolCategories == null)
			{
				throw new KeyNotFoundException("Farm Tool Categories not found.");
			}
			_mapper.Map(farmToolCategoriesRequest, farmToolCategories);
			farmToolCategories.UpdateDate = DateTime.UtcNow;
			_unitOfWork.FarmToolCategoriesRepository.Update(farmToolCategories);
			await _unitOfWork.SaveChangesAsync();
			var farmToolCategoriesMap = _mapper.Map<FarmToolCategoriesResponseDTO>(farmToolCategories);
			farmToolCategoriesMap.FarmToolCategoriesId = farmToolCategories.FarmToolCategoriesId.ToString();
			return farmToolCategoriesMap;
		}
		public async Task<FarmToolCategoriesResponseDTO> UpdateFarmToolCategoryStatusAsync(string FarmToolCategoriesId)
		{
			var FarmToolCategoryId = Guid.Parse(FarmToolCategoriesId);
			var farmToolCategories = await _unitOfWork.FarmToolCategoriesRepository.GetByIdAsync(FarmToolCategoryId);
			if (farmToolCategories == null)
			{
				throw new KeyNotFoundException("Farm Tool Categories not found.");
			}
			farmToolCategories.UpdateDate = DateTime.UtcNow;
			farmToolCategories.Status = false;
			_unitOfWork.FarmToolCategoriesRepository.Update(farmToolCategories);
			await _unitOfWork.SaveChangesAsync();
			var farmToolCategoriesMap = _mapper.Map<FarmToolCategoriesResponseDTO>(farmToolCategories);
			farmToolCategoriesMap.FarmToolCategoriesId = farmToolCategories.FarmToolCategoriesId.ToString();
			return farmToolCategoriesMap;
		}
	}
}
