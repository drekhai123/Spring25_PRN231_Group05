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
        public AutoMapperProfile() {
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<CategoryRequestDTO, Category>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductAddDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();
        }
    }
}
