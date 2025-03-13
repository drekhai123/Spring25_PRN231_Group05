using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IFieldService
    {
        Task<FieldDTO> AddFieldAsync(FieldCreateDTO fieldCreateDTO);
        Task<FieldDTO> UpdateFieldAsync(Guid id, FieldUpdateDTO fieldUpdateDTO);
        Task<bool> DeleteFieldAsync(Guid fieldId);
        Task<FieldDTO> GetFieldByIdAsync(Guid fieldId);
        Task<IEnumerable<FieldDTO>> GetAllFieldsAsync();
    }
} 