using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ProductFieldController : ControllerBase
    {
        private readonly IProductFieldService _productFieldService;

        public ProductFieldController(IProductFieldService productFieldService)
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

        [HttpGet("get-by-{id}")]
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

        [HttpDelete("delete-by-{id}")]
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
        [HttpPost("create-productField")]
        public async Task<ActionResult<ProductFieldResponse>> CreateProductField([FromBody] ProductFieldRequest newproductField)
        {
            try
            {
                var productField = await _productFieldService.CreateProductFieldsAsync(newproductField);
                return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId },
                    productField);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-productfield-by-{id}")]
        public async Task<ActionResult<ProductFieldRequest>> UpdateProductFieldAsync(Guid id, [FromBody] ProductFieldRequest productFieldDto)
        {
            try
            {
                var productField = await _productFieldService.UpdateProductFieldsAsync(id, productFieldDto);
                return Ok(productFieldDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}