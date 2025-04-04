using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
    public class ProductField
    {
        [Key]
        public Guid ProductFieldId { get; set; }
        public double? Productivity { get; set; }
        public string? ProductivityUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public ProductFieldStatus ProductFieldStatus { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public Guid FieldId { get; set; }
        [ForeignKey(nameof(FieldId))]
        public Field? Field { get; set; }
    }
    public enum ProductFieldStatus
    { 
        [EnumMember(Value = "ReadyToPlant")]
        READYTOPLANT,
        [EnumMember(Value = "Growing")]
        GROWING,
        [EnumMember(Value = "ReadyToHarvest")]
        READYTOHARVEST,
        [EnumMember(Value = "Harvesting")]
        HARVESTING,
        [EnumMember(Value = "Harvested")]
        HARVESTED,
        [EnumMember(Value = "Overdue")]
        OVERDUE
    }
}
