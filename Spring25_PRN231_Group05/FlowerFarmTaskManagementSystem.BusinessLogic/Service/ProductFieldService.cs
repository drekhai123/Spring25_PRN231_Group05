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

        public async Task<ProductFieldDTO> AddProductFieldAsync(ProductFieldCreateDTO productFieldCreateDTO)
        {
            // Validate that Product and Field exist
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productFieldCreateDTO.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productFieldCreateDTO.ProductId} not found");

            var field = await _unitOfWork.FieldRepository.GetByIdAsync(productFieldCreateDTO.FieldId);
            if (field == null) throw new KeyNotFoundException($"Field with ID {productFieldCreateDTO.FieldId} not found");

            var productField = _mapper.Map<ProductField>(productFieldCreateDTO);
            productField.ProductFieldId = Guid.NewGuid();
            productField.CreateDate = DateTime.UtcNow;
            productField.UpdateDate = DateTime.UtcNow;
            productField.Status = "";

            await _unitOfWork.ProductFieldRepository.AddAsync(productField);
            await _unitOfWork.SaveChangesAsync();

            // Get the complete ProductField with related entities
            var createdProductField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == productField.ProductFieldId,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            var result = _mapper.Map<ProductFieldDTO>(createdProductField);
            result.CreatedDate = productField.CreateDate;
            result.UpdatedDate = productField.UpdateDate;
            return result;
        }

        public async Task<ProductFieldDTO> UpdateProductFieldAsync(Guid id, ProductFieldUpdateDTO productFieldUpdateDTO)
        {
            var productField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            if (productField == null) throw new KeyNotFoundException($"ProductField with ID {id} not found");

            // Validate that Product and Field exist if they are being updated
            if (productFieldUpdateDTO.ProductId != productField.ProductId)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productFieldUpdateDTO.ProductId);
                if (product == null) throw new KeyNotFoundException($"Product with ID {productFieldUpdateDTO.ProductId} not found");
            }

            if (productFieldUpdateDTO.FieldId != productField.FieldId)
            {
                var field = await _unitOfWork.FieldRepository.GetByIdAsync(productFieldUpdateDTO.FieldId);
                if (field == null) throw new KeyNotFoundException($"Field with ID {productFieldUpdateDTO.FieldId} not found");
            }

            _mapper.Map(productFieldUpdateDTO, productField);
            productField.UpdateDate = DateTime.UtcNow;

            _unitOfWork.ProductFieldRepository.Update(productField);
            await _unitOfWork.SaveChangesAsync();

            // Get the updated ProductField with related entities
            var updatedProductField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            var result = _mapper.Map<ProductFieldDTO>(updatedProductField);
            result.CreatedDate = productField.CreateDate;
            result.UpdatedDate = productField.UpdateDate;
            return result;
        }

        public async Task<bool> DeleteProductFieldAsync(Guid productFieldId)
        {
            var productField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == productFieldId
            ).FirstOrDefault());

            if (productField == null) throw new KeyNotFoundException($"ProductField with ID {productFieldId} not found");

            _unitOfWork.ProductFieldRepository.Delete(productField);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProductFieldDTO> GetProductFieldByIdAsync(Guid id)
        {
            var productField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            if (productField == null)
                throw new KeyNotFoundException($"ProductField with ID {id} not found");

            return _mapper.Map<ProductFieldDTO>(productField);
        }

        public async Task<IEnumerable<ProductFieldDTO>> GetAllProductFieldsAsync()
        {
            var productFields = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                includeProperties: "Product.Category,Field"
            ));
            return _mapper.Map<IEnumerable<ProductFieldDTO>>(productFields);
        }
    }
}