using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
	public class FarmToolsOfTaskRequestDTO
	{
		public String? FarmToolsOfTaskId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
        public int FarmToolOfTaskQuantity { get; set; }
        public string FarmToolOfTaskUnit { get; set; }
        public String FarmToolsId { get; set; }
		public String UserTaskId { get; set; }
		public FarmTools? FarmTools { get; set; }
		public UserTask? UserTask { get; set; }
	}
}
