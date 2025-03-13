using Microsoft.AspNetCore.Mvc;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.APIWithOdata.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ProductFieldsController : ControllerBase
    {
        private readonly IProductFieldService _productFieldService;

        public ProductFieldsController(IProductFieldService productFieldService)
        {
            _productFieldService = productFieldService;
        }

        [HttpGet("get-all-productField")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductFieldRequest>>> GetAllProductFields()
        {
            var productFields = await _productFieldService.GetAllProductFieldsAsync();
            return Ok(productFields);
        }

        [HttpGet("get-by-id")]
        public async Task<ActionResult<ProductFieldRequest>> GetProductFieldById(Guid id)
        {
            try
            {
                var productField = await _productFieldService.GetProductFieldByIdAsync(id);
                return Ok(productField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete-by-id")]
        public async Task<ActionResult<bool>> DeleteProductFieldAsync(Guid id)
        {
            try
            {
                await _productFieldService.DeleteProductFieldsAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
