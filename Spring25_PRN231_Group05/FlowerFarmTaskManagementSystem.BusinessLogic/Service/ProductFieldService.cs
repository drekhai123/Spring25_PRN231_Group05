using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using FlowerFarmTaskManagementSystem.DataAccess.Repositories;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class ProductFieldService : IProductFieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductFieldService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> DeleteProductFieldsAsync(Guid id)
        {
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null) throw new KeyNotFoundException("Product field not found");
            _unitOfWork.ProductFieldRepository.Delete(productField);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

       

        public async Task<IEnumerable<ProductFieldRequest>> GetAllProductFieldsAsync()
        {
            var productFields = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                includeProperties: "Product.Category,Field"
            ));
            return _mapper.Map<IEnumerable<ProductFieldRequest>>(productFields);
        }

        public async Task<ProductFieldRequest> GetProductFieldByIdAsync(Guid id)
        {
            var productField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            if (productField == null)
                throw new KeyNotFoundException($"ProductField with ID {id} not found");

            return _mapper.Map<ProductFieldRequest>(productField);
        }

        public async Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldRequest productFieldRequest)
        {
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
            {
                throw new KeyNotFoundException("ProductField not found.");
            }
            _mapper.Map(productFieldRequest, productField);
            productField.UpdateDate = DateTime.UtcNow;
            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductFieldResponse>(productField);
        }

        public async  Task<ProductFieldResponse> CreateProductFieldsAsync(ProductFieldRequest newPorductField)
        {
            var productField = _mapper.Map<ProductField>(newPorductField);
            productField.ProductFieldId = Guid.NewGuid();
            productField.StartDate = DateTime.UtcNow;
            productField.EndDate = DateTime.UtcNow;
            productField.CreateDate = DateTime.UtcNow;
            productField.UpdateDate = DateTime.UtcNow;
            productField.Status = true;
            await _unitOfWork.ProductFieldRepository.AddAsync(productField);
            await _unitOfWork.SaveChangesAsync();
            return  _mapper.Map<ProductFieldResponse>(productField);
            
        }

        public async  Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string Productivity, string ProductivityUnit)
        {
            throw new NotImplementedException();
        }

     
    }
    }
