using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldDTO>> GetAllProductFieldsAsync();
        Task<ProductFieldDTO> GetProductFieldByIdAsync(Guid id);
    
         Task<ProductFieldDTO> UpdateProductFieldsAsync(Guid id, ProductFieldDTO productField);
         Task<bool> DeleteProductFieldsAsync(Guid id);
         Task<ProductFieldDTO>  CreateProductFieldsAsync(ProductFieldDTO newPorductField);
         Task<IEnumerable<ProductFieldDTO>> SearchProductFieldsAsync(string Productivity, string ProductivityUnit);
    }
}