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

        public async Task<IEnumerable<ProductFieldDetailDTO>> GetAllProductFieldsAsync()
        {
            var productFields = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                includeProperties: "Product.Category,Field"
            ));
            return _mapper.Map<IEnumerable<ProductFieldDetailDTO>>(productFields);
        }

        public async Task<ProductFieldDetailDTO> GetProductFieldByIdAsync(Guid id)
        {
            var productField = await Task.FromResult(_unitOfWork.ProductFieldRepository.Get(
                filter: pf => pf.ProductFieldId == id,
                includeProperties: "Product.Category,Field"
            ).FirstOrDefault());

            if (productField == null)
                throw new KeyNotFoundException($"ProductField with ID {id} not found");

            return _mapper.Map<ProductFieldDetailDTO>(productField);
        }
    }
}