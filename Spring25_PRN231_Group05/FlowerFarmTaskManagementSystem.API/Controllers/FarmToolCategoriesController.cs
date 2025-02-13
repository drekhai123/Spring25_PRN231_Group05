using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
	[Route("odata/[controller]")]
	[ApiController]
	public class FarmToolCategoriesController : ControllerBase
	{
		private readonly IFarmToolCategoriesService _farmToolCategoriesService;
		private readonly IMapper _mapper;
		public FarmToolCategoriesController(IFarmToolCategoriesService farmToolCategoriesService, IMapper mapper)
		{
			_farmToolCategoriesService = farmToolCategoriesService;
			_mapper = mapper;
		}
		[HttpGet("get-all-farm-tool-category")]
		public async Task<ActionResult<IEnumerable<FarmToolCategoriesResponseDTO>>> GetAllFarmToolCategories()
		{
			var farmToolCategories = await _farmToolCategoriesService.GetAllFarmToolCategoriesAsync();
			return Ok(farmToolCategories);
		}
		[HttpGet("get-farm-tool-category-by-id")]
		public async Task<ActionResult<FarmToolCategoriesResponseDTO>> GetFarmToolCategoryById(String FarmToolCategoriesId)
		{
			try
			{
				var farmToolCategories = await _farmToolCategoriesService.GetFarmToolCategoryByIdAsync(FarmToolCategoriesId);
				return Ok(farmToolCategories);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("create-farm-tool-category")]
		public async Task<ActionResult<FarmToolCategoriesRequestDTO>> CreateFarmToolCategory([FromBody] FarmToolCategoriesRequestDTO farmToolCategoriesRequest)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var farmToolCategories = await _farmToolCategoriesService.CreateFarmToolCategoryAsync(farmToolCategoriesRequest);
			return Ok(farmToolCategories);
		}
		[HttpPut("update-farm-tool-category")]
		public async Task<ActionResult<FarmToolCategoriesRequestDTO>> UpdateFarmToolCategory([FromBody] FarmToolCategoriesRequestDTO farmToolCategoriesRequest)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmToolCategories = await _farmToolCategoriesService.UpdateFarmToolCategoryAsync(farmToolCategoriesRequest);
				return Ok(farmToolCategories);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPut("update-farm-tool-category-status")]
		public async Task<ActionResult<FarmToolCategoriesRequestDTO>> UpdateFarmToolCategoryStatusAsync(String FarmToolCategoriesId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var farmToolCategories = await _farmToolCategoriesService.UpdateFarmToolCategoryStatusAsync(FarmToolCategoriesId);
				return Ok(farmToolCategories);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpDelete("delete-farm-tool-category-by-id")]
		public async Task<ActionResult<bool>> DeleteFarmToolCategory(String FarmToolCategoriesId)
		{
			try
			{
				var result = await _farmToolCategoriesService.DeleteFarmToolCategoriesAsync(FarmToolCategoriesId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
