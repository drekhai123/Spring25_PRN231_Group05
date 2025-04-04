//using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
//using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
//using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.OData.Query;

//namespace FlowerFarmTaskManagementSystem.API.Controllers
//{
//    [Route("odata/[controller]")]
//    [ApiController]
//    public class ProductFieldController : ControllerBase
//    {
//        private readonly IProductFieldService _productFieldService;

//        public ProductFieldController(IProductFieldService productFieldService)
//        {
//            _productFieldService = productFieldService;
//        }

//        [HttpGet("get-all-productField")]
//        [EnableQuery]
//        public async Task<ActionResult<IEnumerable<ProductFieldRequest>>> GetAllProductFields()
//        {
//            var productFields = await _productFieldService.GetAllProductFieldsAsync();
//            return Ok(productFields);
//        }

        //[HttpGet("get-by-id")]
        //public async Task<ActionResult<ProductFieldRequest>> GetProductFieldById(Guid id)
        //{
        //    try
        //    {
        //        var productField = await _productFieldService.GetProductFieldByIdAsync(id);
        //        return Ok(productField);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //}

        // POST: odata/ProductField/create-product-field
        //[HttpPost("create-product-field")]
        //public async Task<ActionResult<ProductFieldDTO>> CreateProductField([FromBody] ProductFieldCreateDTO productFieldCreateDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var productField = await _productFieldService.AddProductFieldAsync(productFieldCreateDTO);
        //        return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId }, productField);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        //// PUT: odata/ProductField/update-product-field
        //[HttpPut("update-product-field")]
        //public async Task<ActionResult<ProductFieldDTO>> UpdateProductField(Guid id, [FromBody] ProductFieldUpdateDTO productFieldUpdateDTO)
        //{
        //    if (id != productFieldUpdateDTO.ProductFieldId)
        //    {
        //        return BadRequest(new { Message = "ProductField ID mismatch." });
        //    }

        //    try
        //    {
        //        var updatedProductField = await _productFieldService.UpdateProductFieldAsync(id, productFieldUpdateDTO);
        //        return Ok(updatedProductField);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        //// DELETE: odata/ProductField/delete-product-field
        //[HttpDelete("delete-product-field")]
        //public async Task<ActionResult> DeleteProductField(Guid id)
        //{
        //    try
        //    {
        //        await _productFieldService.DeleteProductFieldAsync(id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        //[HttpDelete("delete-by-id")]
        //public async Task<ActionResult<bool>> DeleteProductFieldAsync(Guid id)
        //{
        //    try
        //    {
        //        await _productFieldService.DeleteProductFieldsAsync(id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //}

        //[HttpPost("create-productField")]
        //public async Task<ActionResult<ProductFieldResponse>> CreateProductField([FromBody] ProductFieldCreateDTO newproductField)
        //{
        //    try
        //    {
        //        var productField = await _productFieldService.CreateProductFieldsAsync(newproductField);
        //        return CreatedAtAction(nameof(GetProductFieldById), new { id = productField.ProductFieldId },
        //            productField);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPut("update-productfield-by-id")]
        public async Task<ActionResult<ProductFieldRequest>> UpdateProductFieldAsync(Guid id, [FromBody] ProductFieldUpdateDTO productFieldDto)
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

//    }
//}