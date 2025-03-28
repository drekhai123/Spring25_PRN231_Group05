﻿using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
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

        // GET: odata/Product/get-all-product
        [HttpGet("get-all-product")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

		// GET: odata/Product/by-id
		[HttpGet("by-id")]
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

        // GET: odata/Product/check-product-in-use
        [HttpGet("check-product-in-use")]
        public async Task<ActionResult<bool>> CheckProductInUse(Guid id)
        {
            try
            {
                var isInUse = await _productService.IsProductInUseAsync(id);
                return Ok(isInUse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // POST: odata/Product
        [HttpPost("add-product")]
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductAddDTO productAddDTO)
        {
            var product = await _productService.AddProductAsync(productAddDTO);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

		// PUT: odata/Product/update-product
		[HttpPut("update-product")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(Guid id, ProductUpdateDTO productUpdateDTO)
        {
            if (id != productUpdateDTO.ProductId)
            {
                return BadRequest(new { Message = "Product ID mismatch." });
            }

            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, productUpdateDTO);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

		// DELETE: odata/Product/delete-product
		[HttpDelete("delete-product")]
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
