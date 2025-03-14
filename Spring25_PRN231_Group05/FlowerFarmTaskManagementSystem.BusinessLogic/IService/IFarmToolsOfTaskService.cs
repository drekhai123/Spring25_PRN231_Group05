using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
	public interface IFarmToolsOfTaskService
	{
		Task<IEnumerable<FarmToolsOfTaskResponseDTO>> GetAllFarmToolsOfTasksAsync();
		Task<FarmToolsOfTaskResponseDTO> GetFarmToolsOfTasksByIdAsync(String FarmToolsOfTasksId);
		Task<List<FarmToolsOfTaskResponseDTO>> CreateFarmToolsOfTasksAsync(CreateFarmToolsOfTaskRequestDTO request);
		Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksAsync(FarmToolsOfTaskRequestDTO request);
		Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksExtendAsync(FarmToolsOfTaskExtendRequestDTO request);
		Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksStatusFinishAsync(String FarmToolsOfTasksId);
		Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksStatusPendingAsync(String FarmToolsOfTasksId);
		Task<bool> DeleteFarmToolsOfTasksAsync(String FarmToolsOfTasksId);
	}
}
