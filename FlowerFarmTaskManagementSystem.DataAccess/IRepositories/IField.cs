using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.DataAccess.IRepositories
{
    public interface IField
    {
        Task<IEnumerable<Field>> GetAllFieldAsync();
        Task<Field> GetFieldByIdAsync(Guid id);
        Task<Field> CreateFieldAsync(Field field);
        Task<Field> UpdateFieldAsync(Guid id, Field field);
        Task<bool> DeleteFieldAsync(Guid id);
    }
}
