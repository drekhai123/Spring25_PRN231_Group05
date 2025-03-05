using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FFTMS.RazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Optional: You can remove this check if you're using middleware to handle redirects
            if (!Request.Cookies.ContainsKey("AuthToken"))
            {
                return RedirectToPage("/Auth/LoginPage");
            }

            return Page();
        }
    }
}
