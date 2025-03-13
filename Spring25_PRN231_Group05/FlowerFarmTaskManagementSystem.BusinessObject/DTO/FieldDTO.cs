using System;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class FieldDTO
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public string Description { get; set; }
        public string? FieldImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }
    }
}