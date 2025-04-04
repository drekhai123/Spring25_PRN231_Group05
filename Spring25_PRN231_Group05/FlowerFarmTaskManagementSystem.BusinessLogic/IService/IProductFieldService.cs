using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldResponse>> GetAllProductFieldsAsync(int pageNumber = 1, int pageSize = 5);
        Task<ProductFieldResponse> GetProductFieldByIdAsync(Guid id);
        Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldUpdateDTO productField);
        Task<ProductFieldResponse> UpdateProductFieldProductivity(string id, double Productivity, string ProductivityUnit);
        Task<bool> DeleteProductFieldsAsync(Guid id);
        Task<ProductFieldResponse> CreateProductFieldsAsync(ProductFieldRequest newProductField);
        Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string productivity, string productivityUnit);
        Task<ProductFieldResponse> IncrementProductFieldStatus(Guid id);
    }
}
