using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class ProductFieldAdd
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }
        public ProductFieldStatus ProductFieldStatus { get; set; }
        public Guid ProductId { get; set; }
        public Guid FieldId { get; set; }

    }
}
