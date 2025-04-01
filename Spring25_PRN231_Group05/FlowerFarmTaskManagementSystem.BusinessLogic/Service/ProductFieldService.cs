using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;

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

        public async Task<ProductFieldResponse> CreateProductFieldsAsync(ProductFieldRequest newProductField)
        {
            if (newProductField == null)
                throw new ArgumentNullException(nameof(newProductField));

            // Validation
            if (newProductField.ProductId == Guid.Empty)
                throw new ArgumentException("ProductId is required.");
            if (newProductField.FieldId == Guid.Empty)
                throw new ArgumentException("FieldId is required.");
            if (string.IsNullOrEmpty(newProductField.ProductivityUnit))
                throw new ArgumentException("ProductivityUnit is required.");
            if (newProductField.CreateDate == default)
                throw new ArgumentException("CreateDate is required.");

            var productField = _mapper.Map<ProductField>(newProductField);
            productField.ProductFieldId = Guid.NewGuid();
            productField.Status = newProductField.Status ?? "Planned";

            // Kiểm tra ProductId và FieldId
            var productExists = await _unitOfWork.ProductRepository.GetByIdAsync(productField.ProductId);
            if (productExists == null)
                throw new KeyNotFoundException($"Product with ID {productField.ProductId} not found.");

            var fieldExists = await _unitOfWork.FieldRepository.GetByIdAsync(productField.FieldId);
            if (fieldExists == null)
                throw new KeyNotFoundException($"Field with ID {productField.FieldId} not found.");

            await _unitOfWork.ProductFieldRepository.AddAsync(productField);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductFieldResponse>(productField);
        }

        public async Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldRequest productFieldRequest)
        {
            if (productFieldRequest == null)
                throw new ArgumentNullException(nameof(productFieldRequest));

            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
                throw new KeyNotFoundException("ProductField not found.");

            // Validation
            if (productFieldRequest.ProductId == Guid.Empty)
                throw new ArgumentException("ProductId is required.");
            if (productFieldRequest.FieldId == Guid.Empty)
                throw new ArgumentException("FieldId is required.");
            if (string.IsNullOrEmpty(productFieldRequest.ProductivityUnit))
                throw new ArgumentException("ProductivityUnit is required.");
            if (productFieldRequest.UpdateDate == default)
                throw new ArgumentException("UpdateDate is required.");

            // Kiểm tra ProductId và FieldId
            var productExists = await _unitOfWork.ProductRepository.GetByIdAsync(productFieldRequest.ProductId);
            if (productExists == null)
                throw new KeyNotFoundException($"Product with ID {productFieldRequest.ProductId} not found.");

            var fieldExists = await _unitOfWork.FieldRepository.GetByIdAsync(productFieldRequest.FieldId);
            if (fieldExists == null)
                throw new KeyNotFoundException($"Field with ID {productFieldRequest.FieldId} not found.");

            _mapper.Map(productFieldRequest, productField);
            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductFieldResponse>(productField);
        }

        public async Task<bool> DeleteProductFieldsAsync(Guid id)
        {
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
                throw new KeyNotFoundException("Product field not found");

            _unitOfWork.ProductFieldRepository.Delete(productField);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<ProductFieldResponse>> GetAllProductFieldsAsync(int pageNumber = 1, int pageSize = 5)
        {
            var productFields = _unitOfWork.ProductFieldRepository
                .Get(includeProperties: "Product.Category,Field")
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return _mapper.Map<IEnumerable<ProductFieldResponse>>(productFields);
        }

        public async Task<ProductFieldResponse> GetProductFieldByIdAsync(Guid id)
        {
            var productField = _unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault();

            if (productField == null)
                throw new KeyNotFoundException($"ProductField with ID {id} not found");

            return _mapper.Map<ProductFieldResponse>(productField);
        }

        public async Task<IEnumerable<ProductFieldResponse>> SearchProductFieldsAsync(string productivity, string productivityUnit)
        {
            var query = _unitOfWork.ProductFieldRepository.Get(includeProperties: "Product.Category,Field");

            if (!string.IsNullOrEmpty(productivity) && double.TryParse(productivity, out var productivityValue))
            {
                query = query.Where(pf => pf.Productivity == productivityValue);
            }

            if (!string.IsNullOrEmpty(productivityUnit))
            {
                query = query.Where(pf => pf.ProductivityUnit != null && pf.ProductivityUnit.Contains(productivityUnit, StringComparison.OrdinalIgnoreCase));
            }

            var productFields = query.ToList();
            return _mapper.Map<IEnumerable<ProductFieldResponse>>(productFields);
        }
    }
}
