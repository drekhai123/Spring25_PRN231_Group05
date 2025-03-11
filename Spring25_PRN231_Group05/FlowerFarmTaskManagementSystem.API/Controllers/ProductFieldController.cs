using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
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

        // GET: odata/ProductField/get-all-product-field
        [HttpGet("get-all-product-field")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductFieldDTO>>> GetAllProductFields()
        {
            var productFields = await _productFieldService.GetAllProductFieldsAsync();
            return Ok(productFields);
        }

        // GET: odata/ProductField/by-id
        [HttpGet("by-id")]
        public async Task<ActionResult<ProductFieldDTO>> GetProductFieldById(Guid id)
        {
            try
            {
                var productField = await _productFieldService.GetProductFieldByIdAsync(id);
                return Ok(productField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // POST: odata/ProductField/create-product-field
        [HttpPost("create-product-field")]
        public async Task<ActionResult<ProductFieldDTO>> CreateProductField([FromBody] ProductFieldCreateDTO productFieldCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productField = await _productFieldService.AddProductFieldAsync(productFieldCreateDTO);
                return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId }, productField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: odata/ProductField/update-product-field
        [HttpPut("update-product-field")]
        public async Task<ActionResult<ProductFieldDTO>> UpdateProductField(Guid id, [FromBody] ProductFieldUpdateDTO productFieldUpdateDTO)
        {
            if (id != productFieldUpdateDTO.ProductFieldId)
            {
                return BadRequest(new { Message = "ProductField ID mismatch." });
            }

            try
            {
                var updatedProductField = await _productFieldService.UpdateProductFieldAsync(id, productFieldUpdateDTO);
                return Ok(updatedProductField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: odata/ProductField/delete-product-field
        [HttpDelete("delete-product-field")]
        public async Task<ActionResult> DeleteProductField(Guid id)
        {
            try
            {
                await _productFieldService.DeleteProductFieldAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}