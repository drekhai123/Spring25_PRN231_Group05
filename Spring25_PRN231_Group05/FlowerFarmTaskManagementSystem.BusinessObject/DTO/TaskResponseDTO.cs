using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class TaskResponseDTO
    {
        public Guid TaskId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ImageUrl { get; set; }

        // Thông tin ProductField và các quan hệ
        public ProductFieldResponseInfo ProductField { get; set; }
        public List<UserTaskResponseDTO> UserTasks { get; set; }
    }

    public class ProductFieldResponseInfo
    {
        public Guid ProductFieldId { get; set; }
        public double Productivity { get; set; }
        public string ProductivityUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }

        // Thông tin Field
        public FieldResponseInfo Field { get; set; }
        // Thông tin Product
        public ProductResponseInfo Product { get; set; }
    }

    public class FieldResponseInfo
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; }
        public string? FieldImageUrl { get; set; }
        public bool Status { get; set; }
    }

    public class ProductResponseInfo
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public bool Status { get; set; }

        // Thông tin Category
        public CategoryResponseInfo Category { get; set; }
    }

    public class CategoryResponseInfo
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? CategoryImageUrl { get; set; }
        public bool Status { get; set; }
    }
}

public class UserTaskDTO
{
    public Guid UserTaskId { get; set; }
    public string UserTaskDescription { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool Status { get; set; }

    // Thông tin User
    public UserTaskResponseDTO User { get; set; }
}

public class ProductFieldDetailDTO
{
    public Guid ProductFieldId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Productivity { get; set; }
    public string ProductivityUnit { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool Status { get; set; }
    public ProductDTO Product { get; set; }
}