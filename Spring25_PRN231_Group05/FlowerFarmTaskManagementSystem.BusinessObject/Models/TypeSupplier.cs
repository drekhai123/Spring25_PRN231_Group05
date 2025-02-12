using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
	public class TypeSupplier
	{
		[Key]
		public Guid TypeSupplierId { get; set; }
		public string TypeSupplierName { get; set; }
		public string TypeSupplierDescription { get; set; }
		public string? TypeSupplierImageUrl { get; set; }
		public DateTime CreateDate { get; set; }
		public bool TypeStatus { get; set; }
	}

}
