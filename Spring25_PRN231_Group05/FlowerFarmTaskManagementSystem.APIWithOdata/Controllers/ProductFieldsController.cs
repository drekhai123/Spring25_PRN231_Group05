using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ProductFieldController : ODataController
    {
        private readonly IProductFieldService _productFieldService;

        public ProductFieldController(IProductFieldService productFieldService)
        {
            _productFieldService = productFieldService;
        }

        // GET: odata/ProductField
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductFieldResponse>>> GetAllProductFields([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var productFields = await _productFieldService.GetAllProductFieldsAsync(pageNumber, pageSize);
                return Ok(productFields);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: odata/ProductField/{id}
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<ActionResult<ProductFieldResponse>> GetProductFieldById(Guid id)
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
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: odata/ProductField
        [HttpPost]
        public async Task<ActionResult<ProductFieldResponse>> CreateProductField([FromBody] ProductFieldAdd newProductField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productField = await _productFieldService.CreateProductFieldsAsync(newProductField);
                return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId }, productField);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
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

        // PUT: odata/ProductField/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductFieldResponse>> UpdateProductField(Guid id, [FromBody] ProductFieldUpdateDTO productFieldRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedProductField = await _productFieldService.UpdateProductFieldsAsync(id, productFieldRequest);
                return Ok(updatedProductField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut("update-product-field-productivity")]
        public async Task<ActionResult<ProductFieldResponse>> UpdateProductFieldProductivity(string id, double Productivity, string ProductivityUnit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedProductField = await _productFieldService.UpdateProductFieldProductivity(id, Productivity, ProductivityUnit);
                return Ok(updatedProductField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: odata/ProductField/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductField(Guid id)
        {
            try
            {
                var result = await _productFieldService.DeleteProductFieldsAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest(new { Message = "Failed to delete ProductField." });
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
