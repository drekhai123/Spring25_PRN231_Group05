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

            // Validate EndDate must be greater than StartDate
            if (newProductField.EndDate <= newProductField.StartDate)
            {
                throw new ArgumentException("EndDate must be greater than StartDate.");
            }

            // Check for overlapping ProductFields in the same time period
            var existingProductFields = await _unitOfWork.ProductFieldRepository
                .FindAsync(pf => pf.Status && 
                                pf.FieldId == newProductField.FieldId &&
                                ((pf.StartDate <= newProductField.EndDate && pf.EndDate >= newProductField.StartDate)));

            if (existingProductFields.Any())
            {
                var overlappingField = existingProductFields.First();
                throw new InvalidOperationException(
                    $"Không thể tạo Kế hoạch trồng mới. Khu vực trồng đã được sử dụng từ {overlappingField.StartDate:dd/MM/yyyy} đến {overlappingField.EndDate:dd/MM/yyyy}");
            }

            var productField = _mapper.Map<ProductField>(newProductField);
            productField.ProductFieldId = Guid.NewGuid();
            productField.ProductFieldStatus = ProductFieldStatus.GROWING;
            productField.Status = true;

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

        public async Task<ProductFieldResponse> UpdateProductFieldsAsync(Guid id, ProductFieldUpdateDTO productFieldRequest)
        {
            if (productFieldRequest == null)
            {
                throw new ArgumentNullException(nameof(productFieldRequest));
            }

            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
            {
                throw new KeyNotFoundException("ProductField not found.");
            }

            // Validation
            if (productFieldRequest.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId is required.");
            }
            if (productFieldRequest.FieldId == Guid.Empty)
            {
                throw new ArgumentException("FieldId is required.");
            }
            if (string.IsNullOrEmpty(productFieldRequest.ProductivityUnit))
            {
                throw new ArgumentException("ProductivityUnit is required.");
            }

            if (productFieldRequest.EndDate <= productFieldRequest.StartDate)
            {
                throw new ArgumentException("EndDate must be greater than StartDate.");
            }
            var fieldExists = await _unitOfWork.FieldRepository.GetByIdAsync(productFieldRequest.FieldId);
            if (fieldExists == null)
            {
                throw new KeyNotFoundException($"Field with ID {productFieldRequest.FieldId} not found.");
            }

            bool isDateChanged = productField.StartDate != productFieldRequest.StartDate || productField.EndDate != productFieldRequest.EndDate;

            if (isDateChanged)
            {
                var existingProductFields = await _unitOfWork.ProductFieldRepository
      .FindAsync(pf => pf.FieldId == fieldExists.FieldId &&
                       pf.ProductFieldId != id && 
                       pf.Status == true && 
                       ((pf.StartDate < productFieldRequest.EndDate && pf.EndDate > productFieldRequest.StartDate)));

                if (existingProductFields.Any())
                {
                    var overlappingField = existingProductFields.First();
                    throw new InvalidOperationException(
                        $"Không thể cập nhật Kế hoạch. Khu có Tên {fieldExists.FieldName} đã được sử dụng từ {overlappingField.StartDate:dd/MM/yyyy} đến {overlappingField.EndDate:dd/MM/yyyy}.");
                }
            }
            var productExists = await _unitOfWork.ProductRepository.GetByIdAsync(productFieldRequest.ProductId);
            if (productExists == null)
            {
                throw new KeyNotFoundException($"Product with ID {productFieldRequest.ProductId} not found.");
            }



            // Check ProductFieldStatus only for Productivity and ProductivityUnit updates
            if (productField.Productivity != productFieldRequest.Productivity ||
                productField.ProductivityUnit != productFieldRequest.ProductivityUnit)
            {
                if (productField.ProductFieldStatus != ProductFieldStatus.HARVESTED)
                {
                    throw new InvalidOperationException("Không thể cập nhật Năng suất và Đơn vị năng suất. Kế hoạch phải ở trạng thái ĐÃ THU HOẠCH.");
                }
            }

            _mapper.Map(productFieldRequest, productField);

            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ProductFieldResponse>(productField);

            return result;
        }
        public async Task<ProductFieldResponse> UpdateProductFieldProductivity(string id, double Productivity, string ProductivityUnit)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var Id = Guid.Parse(id);
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(Id);
            if (productField == null)
            {
                throw new KeyNotFoundException("ProductField not found.");
            }
            productField.Productivity = Productivity;
            productField.ProductivityUnit = ProductivityUnit;
            productField.ProductFieldStatus = ProductFieldStatus.HARVESTED;
           
            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ProductFieldResponse>(productField);

            return result;
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

        public async Task<ProductFieldResponse> UpdateProductFieldStatus(Guid id, ProductFieldStatus newStatus)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ProductField ID is required.");
            }

            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
            {
                throw new KeyNotFoundException("ProductField not found.");
            }

            // Validate status transition
            if (!IsValidStatusTransition(productField.ProductFieldStatus, newStatus))
            {
                throw new InvalidOperationException($"Invalid status transition from {productField.ProductFieldStatus} to {newStatus}");
            }

            productField.ProductFieldStatus = newStatus;
            productField.UpdateDate = DateTime.UtcNow;

            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductFieldResponse>(productField);
        }

        private bool IsValidStatusTransition(ProductFieldStatus currentStatus, ProductFieldStatus newStatus)
        {
            // Check if the new status is exactly one step higher than the current status
            // and that we haven't reached the maximum status (HARVESTED)
            return currentStatus < ProductFieldStatus.HARVESTED && 
                   (int)newStatus == (int)currentStatus + 1;
        }

        public async Task<ProductFieldResponse> IncrementProductFieldStatus(Guid id)
        {
            var productField = await _unitOfWork.ProductFieldRepository.GetByIdAsync(id);
            if (productField == null)
            {
                throw new KeyNotFoundException($"Product Field with ID {id} not found.");
            }

            // Increment the status based on current status
            switch (productField.ProductFieldStatus)
            {
                case ProductFieldStatus.READYTOPLANT:
                    productField.ProductFieldStatus = ProductFieldStatus.GROWING;
                    break;
                case ProductFieldStatus.GROWING:
                    productField.ProductFieldStatus = ProductFieldStatus.READYTOHARVEST;
                    break;
                case ProductFieldStatus.READYTOHARVEST:
                    productField.ProductFieldStatus = ProductFieldStatus.HARVESTED;
                    break;
                default:
                    throw new InvalidOperationException($"Cannot increment status from {productField.ProductFieldStatus}");
            }

            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductFieldResponse>(productField);
        }
    }
}
