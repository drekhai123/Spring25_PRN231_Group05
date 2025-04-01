using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.Models
{
    public class TaskWork
    {
        [Key]
        public Guid TaskWorkId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string AssignedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ImageUrl { get; set; }
        public TaskStatus TaskStatus { get; set; }

        // Navigation properties
        public virtual ICollection<UserTask> UserTasks { get; set; }
        public Guid? ProductFieldId { get; set; }
        public virtual ProductField ProductField { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskStatus
    {
        [EnumMember(Value = "InProgress")]
        INPROGRESS,
        [EnumMember(Value = "Completed")]
        COMPLETED
    }
}

