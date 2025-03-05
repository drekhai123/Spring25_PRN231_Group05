public class TaskRequestDTO
{
    public string JobTitle { get; set; }
    public string Description { get; set; }
    public string AssignedTo { get; set; }
    public string AssignedBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Status { get; set; }
    public string? ImageUrl { get; set; }
}