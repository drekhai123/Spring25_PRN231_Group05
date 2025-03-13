using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
	[Route("odata/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		public IAuthService _service;
		public AuthController(IAuthService service)
		{
			_service = service;
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login(AuthRequestDTO request)
		{
			var result = await _service.LoginAsync(request);

			// Check if the result is null, which indicates a failed login
			if (result == null)
			{
				return BadRequest(new { Message = "Invalid username or password." });
			}
			// If login is successful, return the AuthResponseDTO
			return Ok(result);
		}
	}
}
