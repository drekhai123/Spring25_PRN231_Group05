﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AutoMapper;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Enums;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<CategoryRequestDTO, Category>();

            // User mappings
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // Product mappings
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<ProductAddDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            // Field mappings
            CreateMap<Field, FieldDTO>()
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => Convert.ToDouble(src.Length)))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => Convert.ToDouble(src.Width)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdateDate));
            CreateMap<FieldCreateDTO, Field>()
                .ForMember(dest => dest.FieldId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true));
            CreateMap<FieldUpdateDTO, Field>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // ProductField mappings
            CreateMap<ProductField, ProductFieldRequest>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.Field))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

            // Task mappings
            CreateMap<TaskWork, TaskResponseDTO>()
                .ForMember(dest => dest.TaskWorkId, opt => opt.MapFrom(src => src.TaskWorkId))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ProductFieldId, opt => opt.MapFrom(src => src.ProductFieldId))
                .ForMember(dest => dest.Productivity, opt => opt.MapFrom(src => src.ProductField.Productivity))
                .ForMember(dest => dest.ProductivityUnit, opt => opt.MapFrom(src => src.ProductField.ProductivityUnit))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductField.Product))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.ProductField.Field))
                .ForMember(dest => dest.UserTasks, opt => opt.MapFrom(src => src.UserTasks));

            CreateMap<TaskRequestDTO, TaskWork>()
                .ForMember(dest => dest.ProductFieldId, opt => opt.MapFrom(src => src.ProductFieldId))
                .ForMember(dest => dest.UserTasks, opt => opt.Ignore());

            // UserTask mappings
            CreateMap<UserTask, UserTaskResponseDTO>()
                .ForMember(dest => dest.UserTaskId, opt => opt.MapFrom(src => src.UserTaskId))
                .ForMember(dest => dest.UserTaskDescription, opt => opt.MapFrom(src => src.UserTaskDescription))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TaskWorkId, opt => opt.MapFrom(src => src.TaskWorkId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Task, opt => opt.MapFrom(src => src.TaskWork));

            CreateMap<UserTaskRequestDTO, UserTask>()
                .ForMember(dest => dest.UserTaskId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)UserTaskStatus.Waiting))
                .ForMember(dest => dest.FarmToolsOfTasks, opt => opt.Ignore());

            // FarmTool mappings
            CreateMap<FarmToolCategoriesRequestDTO, FarmToolCategories>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()));
            CreateMap<FarmToolCategories, FarmToolCategoriesResponseDTO>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()));
            CreateMap<FarmToolsRequestDTO, FarmTools>();
            CreateMap<FarmTools, FarmToolsResponseDTO>()
                .ForMember(dest => dest.FarmToolCategoriesId, opt => opt.MapFrom(src => src.FarmToolCategoriesId.ToString()))
                .ForMember(dest => dest.FarmToolsId, opt => opt.MapFrom(src => src.FarmToolsId.ToString()))
                .ForMember(dest => dest.FarmToolCategories, opt => opt.MapFrom(src => src.FarmToolCategories));

            // Task Basic mapping
            CreateMap<TaskWork, TaskBasicResponseDTO>()
                .ForMember(dest => dest.TaskWorkId, opt => opt.MapFrom(src => src.TaskWorkId))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ProductFieldId, opt => opt.MapFrom(src => src.ProductField.ProductFieldId))
                .ForMember(dest => dest.Productivity, opt => opt.MapFrom(src => src.ProductField != null ? src.ProductField.Productivity : 0))
                .ForMember(dest => dest.ProductivityUnit, opt => opt.MapFrom(src => src.ProductField != null ? src.ProductField.ProductivityUnit : null))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductField != null ? src.ProductField.Product : null))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.ProductField != null ? src.ProductField.Field : null));

            CreateMap<FarmToolsOfTask, FarmToolsOfTaskResponseDTO>()
                .ForMember(dest => dest.FarmToolsOfTaskId, opt => opt.MapFrom(src => src.FarmToolsOfTaskId.ToString()))
                .ForMember(dest => dest.FarmToolsId, opt => opt.MapFrom(src => src.FarmToolsId.ToString()))
                .ForMember(dest => dest.UserTaskId, opt => opt.MapFrom(src => src.UserTaskId.ToString()));
        }
    }
}
