using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FieldService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FieldDTO> AddFieldAsync(FieldCreateDTO fieldCreateDTO)
        {
            var field = _mapper.Map<Field>(fieldCreateDTO);
            field.FieldId = Guid.NewGuid();
            field.CreateDate = DateTime.UtcNow;
            field.UpdateDate = DateTime.UtcNow;
            field.Status = true;

            await _unitOfWork.FieldRepository.AddAsync(field);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FieldDTO>(field);
        }

        public async Task<FieldDTO> UpdateFieldAsync(Guid id, FieldUpdateDTO fieldUpdateDTO)
        {
            var field = await _unitOfWork.FieldRepository.GetByIdAsync(id);
            if (field == null) throw new KeyNotFoundException("Field not found.");

            _mapper.Map(fieldUpdateDTO, field);
            field.UpdateDate = DateTime.UtcNow;

            _unitOfWork.FieldRepository.Update(field);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FieldDTO>(field);
        }

        public async Task<bool> DeleteFieldAsync(Guid fieldId)
        {
            var field = await _unitOfWork.FieldRepository.GetByIdAsync(fieldId);
            if (field == null) throw new KeyNotFoundException("Field not found.");

            _unitOfWork.FieldRepository.Delete(field);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<FieldDTO> GetFieldByIdAsync(Guid fieldId)
        {
            var field = await _unitOfWork.FieldRepository.GetByIdAsync(fieldId);
            if (field == null) throw new KeyNotFoundException("Field not found.");

            return _mapper.Map<FieldDTO>(field);
        }

        public async Task<IEnumerable<FieldDTO>> GetAllFieldsAsync()
        {
            var fields = await _unitOfWork.FieldRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FieldDTO>>(fields);
        }
    }
} 