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
		Task<FarmToolCategoriesRequestDTO> CreateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest);
		Task<FarmToolCategoriesRequestDTO> UpdateFarmToolCategoryAsync(FarmToolCategoriesRequestDTO farmToolCategoriesRequest);
		Task<FarmToolCategoriesRequestDTO> UpdateFarmToolCategoryStatusAsync(String FarmToolCategoriesId);
		Task<bool> DeleteFarmToolCategoriesAsync(String FarmToolCategoriesId);
	}
}
