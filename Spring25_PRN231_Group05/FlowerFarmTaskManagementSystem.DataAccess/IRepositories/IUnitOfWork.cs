using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.DataAccess.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Field> FieldRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<ProductField> ProductFieldRepository { get; }
        IGenericRepository<TaskWork> TaskWorkRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<UserTask> UserTaskRepository { get; }
        Task<int> SaveChangesAsync();
        void Save();
    }
}
