using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class FieldRequestDTO
    {
        public string FieldName { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Description { get; set; }
        public string? FieldImageUrl { get; set; }
        public bool Status { get; set; }
    }
}
