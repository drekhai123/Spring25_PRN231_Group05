using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<ProductFieldDTO> AddProductFieldAsync(ProductFieldCreateDTO productFieldCreateDTO);
        Task<ProductFieldDTO> UpdateProductFieldAsync(Guid id, ProductFieldUpdateDTO productFieldUpdateDTO);
        Task<bool> DeleteProductFieldAsync(Guid productFieldId);
        Task<ProductFieldDTO> GetProductFieldByIdAsync(Guid id);
        Task<IEnumerable<ProductFieldDTO>> GetAllProductFieldsAsync();
    }
}