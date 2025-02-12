using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
	public class Supplier
	{
		[Key]
		public Guid SupplierId { get; set; }
		public string SupplierName { get; set; }

		public string SupplierEmail { get; set; }
		public string? SupplierPhone { get; set; }
		public string? SupplierAddress { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string Password { get; set; }
		public DateTime CreateDate { get; set; }
		public bool IsActive { get; set; }
	}

}
