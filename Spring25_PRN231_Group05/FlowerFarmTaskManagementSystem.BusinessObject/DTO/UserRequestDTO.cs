using System.ComponentModel.DataAnnotations;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class UserRequestDTO
    {

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? WorkPosition { get; set; }
        public string? Experience { get; set; }
    }
}