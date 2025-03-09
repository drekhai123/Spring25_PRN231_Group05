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

        // Chỉ giữ các DTO cần thiết
        public List<UserTaskResponseDTO> UserTasks { get; set; }
        public ProductDTO Product { get; set; }
        public FieldDTO Field { get; set; }
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
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public double Productivity { get; set; }
    public string ProductivityUnit { get; set; }
    public bool Status { get; set; }

    // Thông tin Product và Category
    public ProductDTO Product { get; set; }

    // Thông tin Field
    public FieldDTO Field { get; set; }
}