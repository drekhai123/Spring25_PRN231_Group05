using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
    public interface IFieldService
    {
        Task<IEnumerable<Field>> GetAllAsync();
        Task<FieldRequestDTO> GetByIdAsynce(Guid id);
        Task<FieldRequestDTO> CreateFieldAsynce(FieldRequestDTO field);
        Task<FieldRequestDTO> UpdateFieldAsynce(Guid id, FieldRequestDTO field);
        Task<bool> DeleteFieldAsynce(Guid id);
    }
}
