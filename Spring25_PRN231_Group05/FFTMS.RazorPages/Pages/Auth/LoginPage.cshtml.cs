using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace FFTMS.RazorPages.Pages.Auth
{
    public class LoginPageModel : PageModel
    {
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginPageModel(ILogger<LoginPageModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
                client.BaseAddress = new Uri("https://localhost:7207"); // Replace with your API base URL

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
                    // Read the response (assuming the API returns a JWT token in the response)
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        // Store the token in a cookie
                        Response.Cookies.Append("AuthToken", loginResponse.Token, new CookieOptions
                        {
                            HttpOnly = true, // Prevent JavaScript access for security
                            Secure = true,   // Use HTTPS only
                            SameSite = SameSiteMode.Strict,
                            Expires = Input.RememberMe ?
                                DateTimeOffset.Now.AddDays(30) : // Long-lived cookie if "Remember Me" is checked
                                DateTimeOffset.Now.AddHours(1)   // Short-lived cookie otherwise
                        });

                        // Store user role and username in session only if they are not null
                        if (!string.IsNullOrEmpty(loginResponse.Role))
                        {
                            HttpContext.Session.SetString("UserRole", loginResponse.Role);
                        }
                        if (!string.IsNullOrEmpty(loginResponse.UserName))
                        {
                            HttpContext.Session.SetString("UserName", loginResponse.UserName);
                        }

                        _logger.LogInformation("User logged in successfully.");
                        // Redirect to the root URL (/) after successful login
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
                    // Handle API error response
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