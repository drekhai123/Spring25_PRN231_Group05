using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
	public class TypeOfSupplier
	{
		[Key]
		public Guid TypeOfSupplierId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public bool Status { get; set; }
		public Guid TypeSupplierId { get; set; }
		public Guid SupplierId { get; set; }

		[ForeignKey(nameof(TypeSupplierId))]
		public TypeSupplier? TypeSupplier { get; set; }
		[ForeignKey(nameof(SupplierId))]
		public Supplier? Supplier { get; set; }

	}
}
