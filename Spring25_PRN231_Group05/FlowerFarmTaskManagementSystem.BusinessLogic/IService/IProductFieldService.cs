using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IProductFieldService
    {
        Task<IEnumerable<ProductFieldDetailDTO>> GetAllProductFieldsAsync();
        Task<ProductFieldDetailDTO> GetProductFieldByIdAsync(Guid id);
    }
}