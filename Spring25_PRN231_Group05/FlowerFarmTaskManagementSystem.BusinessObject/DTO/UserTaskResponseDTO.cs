namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class UserTaskResponseDTO
    {
        public Guid UserTaskId { get; set; }
        public string UserTaskDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public Guid TaskWorkId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}