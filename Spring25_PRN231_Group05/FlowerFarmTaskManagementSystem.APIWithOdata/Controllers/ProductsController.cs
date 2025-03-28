using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerFarmTaskManagementSystem.APIWithOdata.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /odata/Products
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: /odata/Products(guid)
        [EnableQuery]
        public async Task<SingleResult<ProductDTO>> Get([FromODataUri] Guid key)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(key);
                return SingleResult.Create(new[] { product }.AsQueryable());
            }
            catch (Exception)
            {
                return SingleResult.Create(Enumerable.Empty<ProductDTO>().AsQueryable());
            }
        }

        // POST: /odata/Products
        public async Task<IActionResult> Post([FromBody] ProductAddDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdProduct = await _productService.AddProductAsync(product);
                return Created(createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: /odata/Products(guid)
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<ProductUpdateDTO> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productToUpdate = new ProductUpdateDTO();
                patch.Patch(productToUpdate);

                var updatedProduct = await _productService.UpdateProductAsync(key, productToUpdate);
                return Updated(updatedProduct);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: /odata/Products(guid)
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            try
            {
                await _productService.DeleteProductAsync(key);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Function: /odata/Products/GetProductById(id=guid)
        [HttpGet]
        public async Task<SingleResult<ProductDTO>> GetProductById([FromODataUri] Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return SingleResult.Create(new[] { product }.AsQueryable());
            }
            catch (Exception)
            {
                return SingleResult.Create(Enumerable.Empty<ProductDTO>().AsQueryable());
            }
        }

        // Function: /odata/Products/CheckProductInUse(id=guid)
        [HttpGet]
        public async Task<IActionResult> CheckProductInUse([FromODataUri] Guid id)
        {
            try
            {
                var isInUse = await _productService.IsProductInUseAsync(id);
                return Ok(isInUse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 