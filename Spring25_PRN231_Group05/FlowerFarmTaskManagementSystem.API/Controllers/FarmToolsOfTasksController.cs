﻿using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FarmToolsOfTasksController : ControllerBase
	{
		private readonly IFarmToolsOfTaskService _farmToolsOfTaskService;
		private readonly IMapper _mapper;
		public FarmToolsOfTasksController(IFarmToolsOfTaskService farmToolsOfTaskService, IMapper mapper)
		{
			_farmToolsOfTaskService = farmToolsOfTaskService;
			_mapper = mapper;
		}

		[EnableQuery]
		[HttpGet("get-all-farm-tools-of-task")]
		public async Task<ActionResult<IEnumerable<FarmToolsOfTaskResponseDTO>>> GetAllFarmToolsOfTasksAsync()
		{
			var farmToolsOT = await _farmToolsOfTaskService.GetAllFarmToolsOfTasksAsync();
			return Ok(farmToolsOT);
		}
		[HttpGet("get-farm-tool-of-task-by-id")]
		public async Task<ActionResult<FarmToolsOfTaskResponseDTO>> GetFarmToolsOfTasksByIdAsync(String FarmToolsOfTaskId)
		{
			try
			{
				var farmToolsOT = await _farmToolsOfTaskService.GetFarmToolsOfTasksByIdAsync(FarmToolsOfTaskId);
				return Ok(farmToolsOT);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("create-farm-tools-of-task")]
		public async Task<ActionResult<List<FarmToolsOfTaskResponseDTO>>> CreateFarmToolsOfTasksAsync([FromBody] CreateFarmToolsOfTaskRequestDTO request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var farmToolsOT = await _farmToolsOfTaskService.CreateFarmToolsOfTasksAsync(request);
			return Ok(farmToolsOT);
		}
		[HttpPut("update-farm-tools-of-task")]
		public async Task<ActionResult<FarmToolsOfTaskResponseDTO>> UpdateFarmToolsOfTasksAsync([FromBody] FarmToolsOfTaskRequestDTO request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmToolsOT = await _farmToolsOfTaskService.UpdateFarmToolsOfTasksAsync(request);
				return Ok(farmToolsOT);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPut("update-farm-tools-of-task-excute")]
		public async Task<ActionResult<FarmToolsOfTaskResponseDTO>> UpdateFarmToolsOfTasksExcuteAsync([FromBody] FarmToolsOfTaskRequestDTO request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmToolsOT = await _farmToolsOfTaskService.UpdateFarmToolsOfTasksExcuteAsync(request);
				return Ok(farmToolsOT);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPut("update-farm-tools-of-task-status-finish")]
		public async Task<ActionResult<FarmToolsOfTaskResponseDTO>> UpdateFarmToolsOfTasksStatusFinishAsync(String FarmToolsOfTaskId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmToolsOT = await _farmToolsOfTaskService.UpdateFarmToolsOfTasksStatusFinishAsync(FarmToolsOfTaskId);
				return Ok(farmToolsOT);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		
		[HttpDelete("delete-farm-tools-of-task-by-id")]
		public async Task<ActionResult<bool>> DeleteFarmToolsOfTasksAsync(String FarmToolsOfTaskId)
		{
			try
			{
				var result = await _farmToolsOfTaskService.DeleteFarmToolsOfTasksAsync(FarmToolsOfTaskId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
