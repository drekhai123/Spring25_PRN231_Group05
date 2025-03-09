public class TaskRequestDTO
{
    public string JobTitle { get; set; }
    public string Description { get; set; }
    public string AssignedBy { get; set; }  // UserId của manager
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Status { get; set; }
    public string? ImageUrl { get; set; }

    // Thông tin ProductField và các quan hệ
    public ProductFieldRequestInfo ProductField { get; set; }

    // Danh sách UserTask (1-5 staff)
    public List<UserTaskRequest> UserTasks { get; set; } = new();
}

public class UserTaskRequest
{
    public string AssignedTo { get; set; }  // UserId của staff
    public string UserTaskDescription { get; set; }  // Mô tả công việc cho staff này
}

public class ProductFieldRequestInfo
{
    public Guid ProductFieldId { get; set; }
    public double Productivity { get; set; }
    public string ProductivityUnit { get; set; }

    // Thông tin Field
    public FieldRequestInfo Field { get; set; }
    // Thông tin Product
    public ProductRequestInfo Product { get; set; }
}

public class FieldRequestInfo
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; }
    public string? FieldImageUrl { get; set; }
}

public class ProductRequestInfo
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductImageUrl { get; set; }

    // Thông tin Category
    public CategoryRequestInfo Category { get; set; }
}

public class CategoryRequestInfo
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string? CategoryImageUrl { get; set; }
}

public class UserTaskCreateDTO
{
    public Guid UserId { get; set; }
    public string UserTaskDescription { get; set; }
}