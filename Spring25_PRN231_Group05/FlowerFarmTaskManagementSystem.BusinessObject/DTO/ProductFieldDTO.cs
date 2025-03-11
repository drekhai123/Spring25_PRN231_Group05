using System;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class ProductFieldDTO
    {
        public Guid ProductFieldId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Productivity { get; set; }
        public string ProductivityUnit { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Status { get; set; }
        public Guid ProductId { get; set; }
        public Guid FieldId { get; set; }
        public ProductDTO Product { get; set; }
        public FieldDTO Field { get; set; }
    }
}