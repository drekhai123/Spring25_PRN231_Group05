using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FFTMS.RazorPages.Pages.Auth
{
    public class SignUpPageModel : PageModel
    {
        private readonly ILogger<SignUpPageModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SignUpPageModel(ILogger<SignUpPageModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(25, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 25 characters")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Phone number is required")]
            [RegularExpression(@"^(0[1-9][0-9]{8})$", ErrorMessage = "Invalid Vietnamese phone number (e.g., 0912345678)")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Address is required")]
            public string Address { get; set; }

            public string Role { get; set; } = "User"; // Default role is User

            [Required(ErrorMessage = "Date of Birth is required")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

                // Prepare the signup request payload
                var signupRequest = new
                {
                    username = Input.Username,
                    email = Input.Email,
                    password = Input.Password,
                    phone = Input.Phone,
                    address = Input.Address,
                    role = Input.Role,
                    dateOfBirth = Input.DateOfBirth
                };

                // Send the signup request to the API
                var response = await client.PostAsJsonAsync("/odata/Auth/register", signupRequest);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User registered successfully.");
                    // Redirect to the login page after successful signup
                    return RedirectToPage("./LoginPage");
                }
                else
                {
                    // Handle API error response
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "Registration failed. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attempting to register.");
                ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
                return Page();
            }
        }

        // Class to deserialize API error responses
        public class ErrorResponse
        {
            public string Message { get; set; }
        }
    }
}