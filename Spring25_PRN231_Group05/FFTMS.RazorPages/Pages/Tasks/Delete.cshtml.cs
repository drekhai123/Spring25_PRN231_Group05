using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskResponseDTO Task { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7207/odata/Task/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Task = JsonSerializer.Deserialize<TaskResponseDTO>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Page();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading task: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:7207/odata/Task/{Task.TaskWorkId}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = $"Error deleting task. Status code: {response.StatusCode}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting task: {ex.Message}";
                return Page();
            }
        }
    }
}