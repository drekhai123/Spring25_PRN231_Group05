using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
	public class FarmToolCategories
	{
		[Key]
		public Guid FarmToolCategoriesId { get; set; }
		public String FarmToolCategoriesName { get; set; }
		public String? FarmToolCategoriesDescription { get; set; }
		public bool Status { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
	}

}
