using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
	public class FarmToolCategoriesRequestDTO
	{
		public String? FarmToolCategoriesId { get; set; }
		public String FarmToolCategoriesName { get; set; }
		public String? FarmToolCategoriesDescription { get; set; }
		public bool? Status { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}
