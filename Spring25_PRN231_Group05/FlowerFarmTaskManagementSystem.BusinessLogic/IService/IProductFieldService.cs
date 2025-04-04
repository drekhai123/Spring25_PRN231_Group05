using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldRequest>> GetAllProductFieldsAsync();
        Task<ProductField> GetProductFieldByIdAsync(Guid id);
        Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldRequest productField);
        Task<bool> DeleteProductFieldsAsync(Guid id);
        Task<ProductFieldResponse>  CreateProductFieldsAsync(ProductFieldCreateDTO newPorductField);
        Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string Productivity, string ProductivityUnit);
    }
}
