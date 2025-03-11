using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductService
    {        
        Task<ProductDTO> AddProductAsync(ProductAddDTO productAddDTO);
        Task<ProductDTO> UpdateProductAsync(Guid id, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<ProductDTO> GetProductByIdAsync(Guid productId);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
}

