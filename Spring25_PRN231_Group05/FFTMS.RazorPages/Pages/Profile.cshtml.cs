using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FFTMS.RazorPages.Pages
{
	public class ProfileModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger<ProfileModel> _logger;

		public ProfileModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ProfileModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
		}

		public UserProfileDTO UserProfile { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			// Lấy UserId từ session
			var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
			if (string.IsNullOrEmpty(userId))
			{
				TempData["ErrorMessage"] = "Please log in to access your profile.";
				return RedirectToPage("/Auth/LoginPage");
			}

			// Lấy token từ cookie
			var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];
			if (string.IsNullOrEmpty(token))
			{
				TempData["ErrorMessage"] = "Please log in to access your profile.";
				return RedirectToPage("/Auth/LoginPage");
			}

			// Gọi API để lấy thông tin người dùng
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri("https://localhost:7207");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await client.GetAsync($"/odata/User/{userId}");
			if (response.IsSuccessStatusCode)
			{
				UserProfile = await response.Content.ReadFromJsonAsync<UserProfileDTO>();
				return Page();
			}
			else
			{
				_logger.LogError($"API call failed: {response.StatusCode}");
				TempData["ErrorMessage"] = "Unable to retrieve user profile. Please try again later.";
				return RedirectToPage("/Error");
			}
		}

		public async Task<IActionResult> OnPostUpdateUserAsync(UserRequestDTO userRequest)
		{
			// Lấy UserId từ session
			var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
			if (string.IsNullOrEmpty(userId))
			{
				TempData["ErrorMessage"] = "Please log in to update your profile.";
				return RedirectToPage("/Auth/LoginPage");
			}

			// Lấy token từ cookie
			var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];
			if (string.IsNullOrEmpty(token))
			{
				TempData["ErrorMessage"] = "Please log in to update your profile.";
				return RedirectToPage("/Auth/LoginPage");
			}

			// Gọi API để cập nhật thông tin người dùng
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri("https://localhost:7207");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			// Đảm bảo UserId trong DTO khớp với userId từ session
			userRequest.UserId = Guid.Parse(userId);

			var response = await client.PutAsJsonAsync($"/odata/User/{userId}", userRequest);
			if (response.IsSuccessStatusCode)
			{
				TempData["SuccessMessage"] = "User profile updated successfully.";
				return RedirectToPage();
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				_logger.LogError($"Failed to update user profile: {response.StatusCode}, {errorContent}");
				TempData["ErrorMessage"] = "Unable to update user profile. Please try again later.";
				return Page();
			}
		}
	}

	public class UserProfileDTO
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Role { get; set; }
		public string DateOfBirth { get; set; }
		public string CreateDate { get; set; }
		public bool IsActive { get; set; }
		public string WorkPosition { get; set; }
		public string Experience { get; set; }
	}

	public class UserRequestDTO
	{
		public Guid UserId { get; set; }

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Username is required")]
		public string UserName { get; set; }

		public string Password { get; set; }

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
		[System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		public string Phone { get; set; }
		public string Address { get; set; }

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Role is required")]
		public string Role { get; set; }

		public DateTime? DateOfBirth { get; set; }
		public string WorkPosition { get; set; }
		public string Experience { get; set; }
	}
}