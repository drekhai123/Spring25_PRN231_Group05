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
	public class FarmToolsService : IFarmToolsService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public FarmToolsService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<FarmToolsResponseDTO> CreateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest)
		{
			var farmTools = _mapper.Map<FarmTools>(farmToolsRequest);
			farmTools.FarmToolsId = Guid.NewGuid();
			farmTools.CreateDate = DateTime.UtcNow;
			farmTools.UpdateDate = DateTime.UtcNow;
			farmTools.Status = true;
			farmTools.IsActive = true;
			await _unitOfWork.FarmToolsRepository.AddAsync(farmTools);
			await _unitOfWork.SaveChangesAsync();
			var farmToolsMap = _mapper.Map<FarmToolsResponseDTO>(farmTools);

			return farmToolsMap;
		}
		public async Task<bool> DeleteFarmToolsAsync(string FarmToolsId)
		{
			var FarmToolId = Guid.Parse(FarmToolsId);
			var farmTools = await _unitOfWork.FarmToolsRepository.GetByIdAsync(FarmToolId);
			if (farmTools == null)
			{
				throw new KeyNotFoundException("Farm Tool not found.");
			}
			_unitOfWork.FarmToolsRepository.Delete(farmTools);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}
		public async Task<IEnumerable<FarmToolsResponseDTO>> GetAllFarmToolsAsync()
		{
			var farmTools = await _unitOfWork.FarmToolsRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<FarmToolsResponseDTO>>(farmTools);
		}
		public async Task<FarmToolsResponseDTO> GetFarmToolsByIdAsync(string FarmToolsId)
		{
			var FarmToolId = Guid.Parse(FarmToolsId);
			var farmTools = await _unitOfWork.FarmToolsRepository.GetByIdAsync(FarmToolId);
			if (farmTools == null)
			{
				throw new KeyNotFoundException("Farm Tool not found.");
			}
			var farmToolsMap = _mapper.Map<FarmToolsResponseDTO>(farmTools);
			return farmToolsMap;
		}
		public async Task<FarmToolsResponseDTO> UpdateFarmToolsAsync(FarmToolsRequestDTO farmToolsRequest)
		{
			var FarmToolId = Guid.Parse(farmToolsRequest.FarmToolsId);
			var farmTools = await _unitOfWork.FarmToolsRepository.GetByIdAsync(FarmToolId);
			if (farmTools == null)
			{
				throw new KeyNotFoundException("Farm Tool not found.");
			}
			_mapper.Map(farmToolsRequest, farmTools);
			farmTools.UpdateDate = DateTime.UtcNow;
			_unitOfWork.FarmToolsRepository.Update(farmTools);
			await _unitOfWork.SaveChangesAsync();
			var farmToolsMap = _mapper.Map<FarmToolsResponseDTO>(farmTools);

			return farmToolsMap;
		}
		public async Task<FarmToolsResponseDTO> UpdateFarmToolsStatusAsync(string FarmToolsId)
		{
			var FarmToolId = Guid.Parse(FarmToolsId);
			var farmTools = await _unitOfWork.FarmToolsRepository.GetByIdAsync(FarmToolId);
			if (farmTools == null)
			{
				throw new KeyNotFoundException("Farm Tool not found.");
			}
			farmTools.UpdateDate = DateTime.UtcNow;
			farmTools.Status = false;
			farmTools.IsActive = false;
			_unitOfWork.FarmToolsRepository.Update(farmTools);
			await _unitOfWork.SaveChangesAsync();
			var farmToolsMap = _mapper.Map<FarmToolsResponseDTO>(farmTools);

			return farmToolsMap;
		}
	}
}
