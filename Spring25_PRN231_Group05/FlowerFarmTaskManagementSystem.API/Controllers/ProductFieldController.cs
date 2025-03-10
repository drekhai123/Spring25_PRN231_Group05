using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFieldController : ControllerBase
    {
        private readonly IProductFieldService _productFieldService;

        public ProductFieldController(IProductFieldService productFieldService)
        {
            _productFieldService = productFieldService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductFieldDTO>>> GetAllProductFields()
        {
            var productFields = await _productFieldService.GetAllProductFieldsAsync();
            return Ok(productFields);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductFieldDTO>> GetProductFieldById(Guid id)
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

        [HttpDelete("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<ProductFieldDTO>> CreateProductField([FromBody] ProductFieldDTO newproductField)
        {
            try
            {
                var productField = await _productFieldService.CreateProductFieldsAsync(newproductField);
                return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId }, productField);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductFieldDTO>> UpdateTProductFieldask(Guid id, [FromBody] ProductFieldDTO productFieldDto)
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