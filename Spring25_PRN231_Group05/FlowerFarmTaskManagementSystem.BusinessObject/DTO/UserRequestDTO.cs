namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class UserRequestDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}