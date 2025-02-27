using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
	[Route("odata/[controller]")]
	[ApiController]
	public class FarmToolsController : ControllerBase
	{
		private readonly IFarmToolsService _farmToolsService;
		private readonly IMapper _mapper;
		public FarmToolsController(IFarmToolsService farmToolsService, IMapper mapper)
		{
			_farmToolsService = farmToolsService;
			_mapper = mapper;
		}

        [EnableQuery]
        [HttpGet("get-all-farm-tools")]
		public async Task<ActionResult<IEnumerable<FarmToolsResponseDTO>>> GetAllFarmTools()
		{
			var farmTools = await _farmToolsService.GetAllFarmToolsAsync();
			return Ok(farmTools);
		}
		[HttpGet("get-farm-tool-by-id")]
		public async Task<ActionResult<FarmToolsResponseDTO>> GetFarmToolsById(String FarmToolsId)
		{
			try
			{
				var farmTools = await _farmToolsService.GetFarmToolsByIdAsync(FarmToolsId);
				return Ok(farmTools);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("create-farm-tools")]
		public async Task<ActionResult<FarmToolsRequestDTO>> CreateFarmTools([FromBody] FarmToolsRequestDTO farmToolsRequest)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var farmTools = await _farmToolsService.CreateFarmToolsAsync(farmToolsRequest);
			return Ok(farmTools);
		}
		[HttpPut("update-farm-tools")]
		public async Task<ActionResult<FarmToolsRequestDTO>> UpdateFarmTools([FromBody] FarmToolsRequestDTO farmToolsRequest)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmTools = await _farmToolsService.UpdateFarmToolsAsync(farmToolsRequest);
				return Ok(farmTools);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPut("update-farm-tools-status")]
		public async Task<ActionResult<FarmToolsRequestDTO>> UpdateFarmToolsStatusAsync(String FarmToolsId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmTools = await _farmToolsService.UpdateFarmToolsStatusAsync(FarmToolsId);
				return Ok(farmTools);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpDelete("delete-farm-tools-by-id")]
		public async Task<ActionResult<bool>> DeleteFarmTools(String FarmToolsId)
		{
			try
			{
				var result = await _farmToolsService.DeleteFarmToolsAsync(FarmToolsId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
