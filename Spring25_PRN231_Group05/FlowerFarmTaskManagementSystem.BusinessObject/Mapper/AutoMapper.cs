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
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<CategoryRequestDTO, Category>();

            // Add User
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductAddDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

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
