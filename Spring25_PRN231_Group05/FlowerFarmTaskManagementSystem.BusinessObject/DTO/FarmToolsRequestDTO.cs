using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
	public class FarmToolsRequestDTO
	{
		public String FarmToolsId { get; set; }
		public String FarmToolsName { get; set; }
		public String? FarmToolsDetails { get; set; }
		public int FarmToolsQuantity { get; set; }
		public string FarmToolsUnit { get; set; }
		public bool Status { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public String FarmToolCategoriesId { get; set; }
	}
}
