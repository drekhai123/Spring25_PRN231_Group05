using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldResponse>> GetAllProductFieldsAsync(int pageNumber = 1, int pageSize = 5);
        Task<ProductFieldResponse> GetProductFieldByIdAsync(Guid id);
        Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldRequest productField);
        Task<bool> DeleteProductFieldsAsync(Guid id);
        Task<ProductFieldResponse> CreateProductFieldsAsync(ProductFieldRequest newProductField);
        Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string productivity, string productivityUnit);
    }
}
