using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: odata/Product
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: odata/Product(1)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // POST: odata/Product
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductAddDTO productAddDTO)
        {
            var product = await _productService.AddProductAsync(productAddDTO);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        // PUT: odata/Product(1)
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(Guid id, ProductUpdateDTO productUpdateDTO)
        {
            if (id != productUpdateDTO.ProductId)
            {
                return BadRequest(new { Message = "Product ID mismatch." });
            }

            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(productUpdateDTO);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: odata/Product(1)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
