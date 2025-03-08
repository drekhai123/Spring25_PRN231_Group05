using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
    public class TaskWork
    {
        [Key]
        public Guid TaskWorkId { get; set; }
        public string JobTitle { get; set; }

        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }

		public Guid? ProductFieldId { get; set; }

		[ForeignKey(nameof(ProductFieldId))]
		public ProductField? ProductField { get; set; }
	}
}
