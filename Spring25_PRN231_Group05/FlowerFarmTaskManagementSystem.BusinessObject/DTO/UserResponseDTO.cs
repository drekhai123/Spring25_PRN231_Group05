namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public string? WorkPosition { get; set; }
        public string? Experience { get; set; }

    }
}