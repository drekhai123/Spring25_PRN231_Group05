using System;
using System.Collections.Generic;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class TaskBasicResponseDTO
    {
        public Guid TaskWorkId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string AssignedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? ImageUrl { get; set; }
        public TaskProgressStatus TaskStatus { get; set; }

        // Bỏ List<UserTaskResponseDTO> để tránh vòng lặp

        public Guid ProductFieldId { get; set; }
        public double Productivity { get; set; }
        public string ProductivityUnit { get; set; }
        public ProductFieldStatus ProductFieldStatus { get; set; }
        public ProductDTO Product { get; set; }
        public FieldDTO Field { get; set; }
    }
}