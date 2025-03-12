using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
    public class UserTask
    {
        [Key]
        public Guid UserTaskId { get; set; }
        public string UserTaskDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? Status { get; set; }
        public Guid TaskWorkId { get; set; }
        public string? ImageUrl { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey(nameof(TaskWorkId))]
        public virtual TaskWork TaskWork { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public virtual ICollection<FarmToolsOfTask> FarmToolsOfTasks { get; set; } = new List<FarmToolsOfTask>();
    }
}
