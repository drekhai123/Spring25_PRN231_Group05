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
		Task<FarmToolsResponseDTO> GetFarmToolsByIdAsync(String FarmToolsId);
		Task<FarmToolsResponseDTO> CreateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest);
		Task<FarmToolsResponseDTO> UpdateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest);
		Task<FarmToolsResponseDTO> UpdateFarmToolsStatusAsync(String FarmToolsId);
		Task<bool> DeleteFarmToolsAsync(String FarmToolsId);
	}
}
