using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldDTO>> GetAllProductFieldsAsync();
        Task<ProductFieldDTO> GetProductFieldByIdAsync(Guid id);
    }
}