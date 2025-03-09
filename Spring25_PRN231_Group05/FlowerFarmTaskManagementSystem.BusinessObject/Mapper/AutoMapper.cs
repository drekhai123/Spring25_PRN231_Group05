using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<Category, CategoryResponseInfo>();
            CreateMap<CategoryRequestDTO, Category>();
            CreateMap<CategoryRequestInfo, Category>();

            // User mappings
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // Product mappings
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<Product, ProductResponseInfo>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<ProductAddDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();
            CreateMap<ProductRequestInfo, Product>();

            // Field mappings
            CreateMap<Field, FieldDTO>()
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => Convert.ToDouble(src.Length)))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => Convert.ToDouble(src.Width)));
            CreateMap<Field, FieldResponseInfo>();
            CreateMap<FieldRequestInfo, Field>();

            // ProductField mappings
            CreateMap<ProductField, ProductFieldDetailDTO>();
            CreateMap<ProductField, ProductFieldResponseInfo>()
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.Field))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
            CreateMap<ProductFieldRequestInfo, ProductField>();

            // Task mappings
            CreateMap<TaskWork, TaskResponseDTO>()
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskWorkId));
            CreateMap<TaskRequestDTO, TaskWork>()
                .ForMember(dest => dest.ProductFieldId, opt => opt.MapFrom(src => src.ProductField.ProductFieldId))
                .ForMember(dest => dest.UserTasks, opt => opt.Ignore())
                .ForMember(dest => dest.ProductField, opt => opt.Ignore());

            // UserTask mappings
            CreateMap<UserTask, UserTaskDTO>();
            CreateMap<UserTask, UserTaskResponseDTO>()
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.TaskWork.JobTitle))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<UserTaskRequestDTO, UserTask>()
                .ForMember(dest => dest.UserTaskId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());
            CreateMap<UserTaskRequest, UserTask>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.AssignedTo)));

            // FarmTool mappings
            CreateMap<FarmToolCategoriesRequestDTO, FarmToolCategories>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()));
            CreateMap<FarmToolCategories, FarmToolCategoriesResponseDTO>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()));
            CreateMap<FarmToolsRequestDTO, FarmTools>();
            CreateMap<FarmTools, FarmToolsResponseDTO>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()))
                .ForMember(dest => dest.FarmToolsId, opt => opt.MapFrom(src => src.FarmToolsId.ToString()));
        }
    }
}
