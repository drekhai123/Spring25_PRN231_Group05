using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
	public interface IFarmToolCategoriesService
	{
		Task<IEnumerable<FarmToolCategoriesResponseDTO>> GetAllFarmToolCategoriesAsync();
		Task<FarmToolCategoriesResponseDTO> GetFarmToolCategoryByIdAsync(String FarmToolCategoriesId);
		Task<FarmToolCategoriesResponseDTO> CreateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest);
		Task<FarmToolCategoriesResponseDTO> UpdateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest);
		Task<FarmToolCategoriesResponseDTO> UpdateFarmToolCategoryStatusAsync(String FarmToolCategoriesId);
		Task<bool> DeleteFarmToolCategoriesAsync(String FarmToolCategoriesId);
	}
}
