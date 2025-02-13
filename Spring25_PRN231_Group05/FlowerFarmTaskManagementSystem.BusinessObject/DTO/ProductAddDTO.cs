using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class ProductAddDTO
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid CategoryId { get; set; }
    }
}
