using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldRequest>> GetAllProductFieldsAsync();
        Task<ProductFieldRequest> GetProductFieldByIdAsync(Guid id);
        Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldRequest productField);
        Task<bool> DeleteProductFieldsAsync(Guid id);
        Task<ProductFieldResponse>  CreateProductFieldsAsync(ProductFieldRequest newPorductField);
        Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string Productivity, string ProductivityUnit);
    }
}

