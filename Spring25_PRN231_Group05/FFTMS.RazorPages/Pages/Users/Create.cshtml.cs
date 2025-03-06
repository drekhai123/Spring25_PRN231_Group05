using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlowerFarmTaskManagementSystem.BusinessObject.Enums;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public UserRequestDTO User { get; set; } = new UserRequestDTO();

        public IEnumerable<SelectListItem> RoleList => Enum.GetValues<Role>()
            .Select(r => new SelectListItem
            {
                Value = r.ToString(),
                Text = r.ToString()
            });

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
                if (role != "Admin")
                {
                    return RedirectToPage("/Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
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
            if (role != "Admin")
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("odata/User", User);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating user.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}