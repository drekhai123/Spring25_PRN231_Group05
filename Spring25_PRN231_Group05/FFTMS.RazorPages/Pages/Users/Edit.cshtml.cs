using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public UserRequestDTO User { get; set; } = new UserRequestDTO();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"odata/User/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<UserResponseDTO>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Map từ UserResponseDTO sang UserRequestDTO
            User = new UserRequestDTO
            {
                UserId = userResponse.UserId,  // Có thể map trực tiếp
                UserName = userResponse.UserName,
                Email = userResponse.Email,
                Phone = userResponse.Phone,
                Address = userResponse.Address,
                Role = userResponse.Role,
                DateOfBirth = userResponse.DateOfBirth
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage);
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return Page();
                }

                // Log request data
                var jsonRequest = JsonSerializer.Serialize(User);
                Console.WriteLine($"Request data: {jsonRequest}");

                var response = await _httpClient.PutAsJsonAsync($"odata/User/{User.UserId}", User);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating user: {responseContent}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
                return Page();
            }
        }
    }
}