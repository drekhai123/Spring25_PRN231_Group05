using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class ProductFieldUpdateDTO
    {
        public double? Productivity { get; set; }
        public string? ProductivityUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProductFieldStatus ProductFieldStatus { get; set; }
        public Guid ProductId { get; set; }
        public Guid FieldId { get; set; }


    }
} 