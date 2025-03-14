using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class FarmToolsOfTaskExtendRequestDTO
    {
        public String? FarmToolsOfTaskId { get; set; }
        public DateTime EndDate { get; set; }
        public int FarmToolOfTaskQuantity { get; set; }
        public string FarmToolOfTaskUnit { get; set; }
    }
}
