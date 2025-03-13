using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
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
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDTO> AddProductAsync(ProductAddDTO productAddDTO)
        {
            var product = _mapper.Map<Product>(productAddDTO);
            product.ProductId = Guid.NewGuid();
            product.CreateDate = DateTime.UtcNow;
            product.UpdateDate = DateTime.UtcNow;
            product.Status = true;

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }

    
        public async Task<ProductDTO> UpdateProductAsync(Guid id, ProductUpdateDTO productUpdateDTO)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _mapper.Map(productUpdateDTO, product);
            product.UpdateDate = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            if (product == null) throw new KeyNotFoundException("Product not found.");

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<bool> IsProductInUseAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            // Check if the product is used in any ProductField
            var productFields = await _unitOfWork.ProductFieldRepository
                .FindAsync(pf => pf.ProductId == productId && pf.Status.Equals(true));

            return productFields.Any();
        }
    }
}
