using FlowerFarmTaskManagementSystem.BusinessObject.Enums;

public class UserTaskRequestDTO
{
    public UserTaskStatus Status { get; set; }
    public List<Guid> FarmToolIds { get; set; } = new();
}