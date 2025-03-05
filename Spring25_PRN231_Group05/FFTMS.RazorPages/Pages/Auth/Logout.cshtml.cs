using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FFTMS.RazorPages.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Delete the AuthToken cookie
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Auth/LoginPage");
        }
    }
}
