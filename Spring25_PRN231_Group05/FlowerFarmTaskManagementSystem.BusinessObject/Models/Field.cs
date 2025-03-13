using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
    public class Field
    {
        [Key]
        public Guid FieldId { get; set; }
        public string FieldName { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public string Description { get; set; }
        public string? FieldImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
    }
}
