using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
	public interface IFarmToolsService
	{
		Task<IEnumerable<FarmToolsResponseDTO>> GetAllFarmToolsAsync();
		Task<FarmToolsResponseDTO> GetFarmToolsByIdAsync(String FarmToolCategoriesId);
		Task<FarmToolsRequestDTO> CreateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest);
		Task<FarmToolsRequestDTO> UpdateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest);
		Task<FarmToolsRequestDTO> UpdateFarmToolsStatusAsync(String FarmToolsId);
		Task<bool> DeleteFarmToolsAsync(String FarmToolsId);
	}
}
