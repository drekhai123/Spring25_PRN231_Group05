using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class UserTaskFarmToolsResponseDTO
    {
        public Guid UserTaskId { get; set; }
        public string UserTaskDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Status { get; set; }
        public Guid TaskWorkId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public TaskBasicResponseDTO Task { get; set; }
        public IList<FarmToolsOfTaskResponseDTO> FarmToolsOfTask { get; set; }

    }
}
