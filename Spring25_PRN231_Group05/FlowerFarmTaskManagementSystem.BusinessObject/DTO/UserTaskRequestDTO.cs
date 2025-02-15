public class UserTaskRequestDTO
{
    public string UserTaskDescription { get; set; }
    public Guid TaskWorkId { get; set; }
    public Guid UserId { get; set; }
    public bool Status { get; set; }
}