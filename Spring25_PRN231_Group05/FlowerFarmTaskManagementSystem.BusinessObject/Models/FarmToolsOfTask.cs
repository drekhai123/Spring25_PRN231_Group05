using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
	public class FarmToolsOfTask
	{
		[Key]
		public Guid FarmToolsOfTaskId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int FarmToolOfTaskQuantity { get; set; }
		public string FarmToolOfTaskUnit { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
		public Guid FarmToolsId { get; set; }
		[ForeignKey(nameof(FarmToolsId))]
		public FarmTools? FarmTools { get; set; }
        public bool? IsActive { get; set; }
        public Guid UserTaskId { get; set; }
        [ForeignKey(nameof(UserTaskId))]
        public UserTask? UserTask { get; set; }
    }
}
