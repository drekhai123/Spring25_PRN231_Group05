using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Security.Claims;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskRequestDTO Task { get; set; } = new TaskRequestDTO();
        public SelectList UserList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Auth/LoginPage");
                }

                var role = JwtHelper.GetRoleFromToken(token);
                if (role != "Manager")
                {
                    return RedirectToPage("/Index");
                }

                await LoadUserList();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetAsync: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
            }
        }

        private async Task LoadUserList()
        {
            try
            {
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                Console.WriteLine($"API Response Status: {userResponse.StatusCode}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userJsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(userJsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (users != null && users.Any())
                    {
                        var staffUsers = users.Where(u => u.Role == "Staff").ToList();
                        Console.WriteLine($"Found {staffUsers.Count} staff users");

                        var usersList = staffUsers.Select(u => new SelectListItem
                        {
                            Value = u.Email,
                            Text = $"{u.UserName} ({u.Email})"
                        });

                        UserList = new SelectList(usersList, "Value", "Text");
                    }
                }
                else
                {
                    var errorContent = await userResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error loading users: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/LoginPage");
            }

            var role = JwtHelper.GetRoleFromToken(token);
            if (role != "Manager")
            {
                return RedirectToPage("/Index");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadUserList();
                    return Page();
                }

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7207/odata/Task", Task);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Error creating task.");
                    await LoadUserList();
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                await LoadUserList();
                return Page();
            }
        }
    }
}