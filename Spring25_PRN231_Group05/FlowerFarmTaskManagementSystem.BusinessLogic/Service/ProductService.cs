using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class ProductService : IProductService
    {
        private readonly FlowerFarmTaskManagementSystemDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductService(FlowerFarmTaskManagementSystemDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ProductDTO> AddProductAsync(ProductAddDTO productAddDTO)
        {
            var product = _mapper.Map<Product>(productAddDTO);
            product.ProductId = Guid.NewGuid();
            product.CreateDate = DateTime.UtcNow;
            product.UpdateDate = DateTime.UtcNow;
            product.Status = true;

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> UpdateProductAsync(ProductUpdateDTO productUpdateDTO)
        {
            var product = await _dbContext.Products.FindAsync(productUpdateDTO.ProductId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _mapper.Map(productUpdateDTO, product);
            product.UpdateDate = DateTime.UtcNow;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid productId)
        {
            var product = await _dbContext.Products
                .Include(p => p.Category) // Include related Category if needed
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) throw new KeyNotFoundException("Product not found.");

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _dbContext.Products
                .Include(p => p.Category) // Include related Category if needed
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
    }
}
