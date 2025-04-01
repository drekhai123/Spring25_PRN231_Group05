using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FFTMS.RazorPages.Pages.Auth
{
	public class LoginPageModel : PageModel
	{
		private readonly ILogger<LoginPageModel> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration; // Thêm IConfiguration để lấy JWT:Secret

		public LoginPageModel(ILogger<LoginPageModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		[TempData]
		public string ErrorMessage { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Username or Email is required")]
			[Display(Name = "Username or Email")]
			public string UsernameOrEmail { get; set; }

			[Required(ErrorMessage = "Password is required")]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public IActionResult OnGet(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			ReturnUrl = returnUrl ?? Url.Content("~/");
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				// Create an HTTP client
				var client = _httpClientFactory.CreateClient();
				client.BaseAddress = new Uri("https://localhost:7207");

				// Prepare the login request payload
				var loginRequest = new
				{
					usernameOrEmail = Input.UsernameOrEmail,
					password = Input.Password
				};

				// Send the login request to the API
				var response = await client.PostAsJsonAsync("/odata/Auth/login", loginRequest);

				if (response.IsSuccessStatusCode)
				{
					// Read the response
					var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
					_logger.LogInformation($"Token from API: {loginResponse?.Token}");

					if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
					{
						// Giải mã token để lấy UserId
						string userId = null;
						try
						{
							var secret = _configuration["JWT:Secret"];
							if (string.IsNullOrEmpty(secret))
							{
								_logger.LogError("JWT:Secret is not configured in appsettings.json");
								ModelState.AddModelError(string.Empty, "Server configuration error: JWT Secret is missing.");
								return Page();
							}

							var tokenHandler = new JwtSecurityTokenHandler();
							var key = Encoding.UTF8.GetBytes(secret);
							_logger.LogInformation($"JWT Secret: {secret}");

							var validationParameters = new TokenValidationParameters
							{
								ValidateIssuerSigningKey = true,
								IssuerSigningKey = new SymmetricSecurityKey(key),
								ValidateIssuer = false,
								ValidateAudience = false,
								ValidateLifetime = false
							};

							var principal = tokenHandler.ValidateToken(loginResponse.Token, validationParameters, out var validatedToken);
							userId = principal.FindFirst("UserId")?.Value;
							_logger.LogInformation($"Extracted UserId: {userId}");

							// Log tất cả các claim trong token
							foreach (var claim in principal.Claims)
							{
								_logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
							}
						}
						catch (SecurityTokenNoExpirationException ex)
						{
							_logger.LogError(ex, "Token is missing an expiration time.");
							ModelState.AddModelError(string.Empty, "Invalid token: Missing expiration time. Please contact support.");
							return Page();
						}
						catch (SecurityTokenExpiredException ex)
						{
							_logger.LogError(ex, "Token has expired.");
							ModelState.AddModelError(string.Empty, "The token has expired. Please try logging in again.");
							return Page();
						}
						catch (SecurityTokenInvalidSignatureException ex)
						{
							_logger.LogError(ex, "Invalid token signature.");
							ModelState.AddModelError(string.Empty, "Invalid token signature. Please contact support.");
							return Page();
						}
						catch (SecurityTokenInvalidIssuerException ex)
						{
							_logger.LogError(ex, "Invalid issuer.");
							ModelState.AddModelError(string.Empty, "Invalid token issuer.");
							return Page();
						}
						catch (SecurityTokenInvalidAudienceException ex)
						{
							_logger.LogError(ex, "Invalid audience.");
							ModelState.AddModelError(string.Empty, "Invalid token audience.");
							return Page();
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Error decoding token to extract UserId.");
							ModelState.AddModelError(string.Empty, "Invalid token received from API.");
							return Page();
						}

						if (string.IsNullOrEmpty(userId))
						{
							ModelState.AddModelError(string.Empty, "UserId not found in token.");
							return Page();
						}

						// Store the token in a cookie
						Response.Cookies.Append("AuthToken", loginResponse.Token, new CookieOptions
						{
							HttpOnly = true,
							Secure = true,
							SameSite = SameSiteMode.Strict,
							Expires = Input.RememberMe ?
								DateTimeOffset.Now.AddDays(30) :
								DateTimeOffset.Now.AddHours(1)
						});

						// Store user role, username, and userId in session
						if (!string.IsNullOrEmpty(loginResponse.Role))
						{
							HttpContext.Session.SetString("UserRole", loginResponse.Role);
						}
						if (!string.IsNullOrEmpty(loginResponse.UserName))
						{
							HttpContext.Session.SetString("UserName", loginResponse.UserName);
						}
						HttpContext.Session.SetString("UserId", userId);

						_logger.LogInformation("User logged in successfully.");
						return LocalRedirect("/");
					}
					else
					{
						ModelState.AddModelError(string.Empty, "Invalid login response from API.");
						return Page();
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
					ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "Wrong password !! Try again!!.");
					return Page();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while attempting to log in.");
				ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
				return Page();
			}
		}
	}

	// Class to deserialize the API's login response
	public class LoginResponse
	{
		public string Token { get; set; }
		public string Role { get; set; }
		public string UserName { get; set; }
	}

	// Class to deserialize API error responses
	public class ErrorResponse
	{
		public string Message { get; set; }
	}
}