using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
	public class FarmToolsOfTaskService : IFarmToolsOfTaskService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public FarmToolsOfTaskService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

        public async Task<List<FarmToolsOfTaskResponseDTO>> CreateFarmToolsOfTasksAsync(CreateFarmToolsOfTaskRequestDTO request)
        {
            var responseList = new List<FarmToolsOfTaskResponseDTO>();

            if (request.ListFarmTools == null || !request.ListFarmTools.Any())
            {
                throw new Exception("ListFarmTools cannot be null or empty.");
            }

            foreach (var farmTool in request.ListFarmTools)
            {
                if (!Guid.TryParse(farmTool.FarmToolsId, out var farmToolsId))
                {
                    throw new Exception($"Invalid FarmToolsId: {farmTool.FarmToolsId}");
                }

                var farmTools = await _unitOfWork.FarmToolsRepository.GetByIdAsync(farmToolsId);
                if (farmTools == null)
                {
                    throw new Exception($"Farm tool with ID {farmToolsId} not found.");
                }

                if (farmTools.FarmToolsQuantity < farmTool.Quantity)
                {
                    throw new Exception($"Not enough farm tools for ID {farmToolsId}. Available: {farmTools.FarmToolsQuantity}, Requested: {farmTool.Quantity}");
                }

                var farmToolsOfTask = new FarmToolsOfTask
                {
                    FarmToolsOfTaskId = Guid.NewGuid(),
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    Status = 1,
                    FarmToolOfTaskQuantity = farmTool.Quantity,
                    FarmToolOfTaskUnit = request.FarmToolOfTaskUnit,
                    FarmToolsId = farmToolsId,
                    UserTaskId = Guid.Parse(request.UserTaskId)
                };

                await _unitOfWork.FarmToolsOfTaskRepository.AddAsync(farmToolsOfTask);
                responseList.Add(_mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask));
            }

            await _unitOfWork.SaveChangesAsync();
            return responseList;
        }



        public async Task<bool> DeleteFarmToolsOfTasksAsync(string FarmToolsOfTasksId)
		{
			var farmToolsOfTaskId = Guid.Parse(FarmToolsOfTasksId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null)
			{
				throw new KeyNotFoundException("Farm Tool Of Task not found.");
			}
			_unitOfWork.FarmToolsOfTaskRepository.Delete(farmToolsOfTask);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<FarmToolsOfTaskResponseDTO>> GetAllFarmToolsOfTasksAsync()
		{
			var farmToolsOfTasks = await _unitOfWork.FarmToolsOfTaskRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<FarmToolsOfTaskResponseDTO>>(farmToolsOfTasks);
		}

		public async Task<FarmToolsOfTaskResponseDTO> GetFarmToolsOfTasksByIdAsync(string FarmToolsOfTasksId)
		{
			var farmToolsOfTaskId = Guid.Parse(FarmToolsOfTasksId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null)
			{
				throw new KeyNotFoundException("Farm Tool Of Task not found.");
			}
			var farmToolsMap = _mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask);
			return farmToolsMap;
		}

		public async Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksAsync(FarmToolsOfTaskRequestDTO request)
		{
			var farmToolsOfTaskId = Guid.Parse(request.FarmToolsOfTaskId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null) throw new KeyNotFoundException("FarmToolsOfTask not found.");

			_mapper.Map(request, farmToolsOfTask);
			farmToolsOfTask.UpdateDate = DateTime.UtcNow;

			_unitOfWork.FarmToolsOfTaskRepository.Update(farmToolsOfTask);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask);
		}

		public async Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksExtendAsync(FarmToolsOfTaskRequestDTO request)
		{
			var farmToolsOfTaskId = Guid.Parse(request.FarmToolsOfTaskId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null) throw new KeyNotFoundException("FarmToolsOfTask not found.");

			_mapper.Map(request, farmToolsOfTask);
			farmToolsOfTask.UpdateDate = DateTime.UtcNow;
			farmToolsOfTask.Status = 2;

			_unitOfWork.FarmToolsOfTaskRepository.Update(farmToolsOfTask);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask);
		}

		public async Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksStatusFinishAsync(string FarmToolsOfTasksId)
		{
			var farmToolsOfTaskId = Guid.Parse(FarmToolsOfTasksId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null) throw new KeyNotFoundException("FarmToolsOfTask not found.");

			farmToolsOfTask.UpdateDate = DateTime.UtcNow;
			farmToolsOfTask.Status = 3;

			_unitOfWork.FarmToolsOfTaskRepository.Update(farmToolsOfTask);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask);
		}

		public async Task<FarmToolsOfTaskResponseDTO> UpdateFarmToolsOfTasksStatusPendingAsync(string FarmToolsOfTasksId)
		{
			var farmToolsOfTaskId = Guid.Parse(FarmToolsOfTasksId);
			var farmToolsOfTask = await _unitOfWork.FarmToolsOfTaskRepository.GetByIdAsync(farmToolsOfTaskId);
			if (farmToolsOfTask == null) throw new KeyNotFoundException("FarmToolsOfTask not found.");

			farmToolsOfTask.UpdateDate = DateTime.UtcNow;
			farmToolsOfTask.Status = 1;

			_unitOfWork.FarmToolsOfTaskRepository.Update(farmToolsOfTask);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<FarmToolsOfTaskResponseDTO>(farmToolsOfTask);
		}

	}
}
