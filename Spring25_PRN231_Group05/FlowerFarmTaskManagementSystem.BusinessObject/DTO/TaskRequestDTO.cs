public class TaskRequestDTO
{
    public string JobTitle { get; set; }
    public string Description { get; set; }
    public string AssignedBy { get; set; }  // UserId của manager
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Status { get; set; }
    public string? ImageUrl { get; set; }

    // Chỉ cần ProductFieldId
    public Guid ProductFieldId { get; set; }

    // Danh sách UserTask (1-5 staff)
    public List<UserTaskRequest> UserTasks { get; set; } = new();
}

public class UserTaskRequest
{
    public string AssignedTo { get; set; }  // UserId của staff
    public string UserTaskDescription { get; set; }  // Mô tả công việc cho staff này
}



public class UserTaskCreateDTO
{
    public Guid UserId { get; set; }
    public string UserTaskDescription { get; set; }
}