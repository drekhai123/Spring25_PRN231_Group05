using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
	public class CreateFarmToolsOfTaskRequestDTO
	{
		public String? FarmToolsOfTaskId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
        public int FarmToolOfTaskQuantity { get; set; }
        public string? FarmToolOfTaskUnit { get; set; }
        public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public String FarmToolsId { get; set; }
		public String UserTaskId { get; set; }
		public List<FarmToolQuantityDTO>? ListFarmTools { get; set; }
		public UserTask? UserTask { get; set; }
	}
}
