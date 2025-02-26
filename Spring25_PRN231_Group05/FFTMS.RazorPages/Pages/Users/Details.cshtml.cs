using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public UserResponseDTO User { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"odata/User/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    User = JsonSerializer.Deserialize<UserResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Page();
                }
                else
                {
                    ErrorMessage = $"Không thể tải thông tin người dùng. API trả về mã lỗi: {response.StatusCode}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi tải thông tin người dùng: {ex.Message}";
                return Page();
            }
        }
    }
}
