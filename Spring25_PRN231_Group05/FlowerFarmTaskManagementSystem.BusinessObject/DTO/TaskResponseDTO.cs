using System;
using System.Collections.Generic;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class TaskResponseDTO
    {
        public Guid TaskWorkId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? ImageUrl { get; set; }

        public List<UserTaskResponseDTO> UserTasks { get; set; } = new();

        public Guid ProductFieldId { get; set; }
        public double Productivity { get; set; }
        public string ProductivityUnit { get; set; }
        public ProductDTO Product { get; set; }
        public FieldDTO Field { get; set; }
    }
}