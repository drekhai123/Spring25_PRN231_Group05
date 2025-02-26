using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FlowerFarmTaskManagementSystemDbContext _context;
        public UnitOfWork(FlowerFarmTaskManagementSystemDbContext context)
        {
            _context = context;
        }
        private IGenericRepository<Category> _categories;
        private IGenericRepository<Field> _fields;
        private IGenericRepository<Product> _products;
        private IGenericRepository<ProductField> _productFields;
        private IGenericRepository<TaskWork> _taskWorks;
        private IGenericRepository<User> _users;
        private IGenericRepository<UserTask> _userTasks;
		private IGenericRepository<FarmToolCategories> _farmToolCategories;
		private IGenericRepository<FarmTools> _farmTools;

		public IGenericRepository<Category> CategoryRepository => _categories ??= new GenericRepository<Category>(_context);
        public IGenericRepository<Field> FieldRepository => _fields ??= new GenericRepository<Field>(_context);
        public IGenericRepository<Product> ProductRepository => _products ??= new GenericRepository<Product>(_context);
        public IGenericRepository<ProductField> ProductFieldRepository => _productFields ??= new GenericRepository<ProductField>(_context);
        public IGenericRepository<TaskWork> TaskWorkRepository => _taskWorks ??= new GenericRepository<TaskWork>(_context);
        public IGenericRepository<User> UserRepository => _users ??= new GenericRepository<User>(_context);
        public IGenericRepository<UserTask> UserTaskRepository => _userTasks ??= new GenericRepository<UserTask>(_context);
		public IGenericRepository<FarmToolCategories> FarmToolCategoriesRepository => _farmToolCategories ??= new GenericRepository<FarmToolCategories>(_context);
		public IGenericRepository<FarmTools> FarmToolsRepository => _farmTools ??= new GenericRepository<FarmTools>(_context);
		public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
