using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace FFTMS.RazorPages.Pages.FarmToolsOfTask
{
    public class ReturnPageModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public ReturnPageModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public FarmToolsOfTaskResponseDTO FarmToolsOfTaskRequest { get; set; } = default!;
        [BindProperty]
        public string NoteInf { get; set; } = string.Empty;
        [BindProperty]
        public int Quantity { get; set; } = 0;
        public async Task<IActionResult> OnGetAsync(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var apiUrl = $"https://localhost:7207/api/FarmToolsOfTasks/get-all-farm-tools-of-task?$filter=FarmToolsOfTaskId eq '{id}'";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<List<FarmToolsOfTaskResponseDTO>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            FarmToolsOfTaskRequest = parsedResponse?.FirstOrDefault();
            if (FarmToolsOfTaskRequest == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(FarmToolsOfTaskRequest?.FarmToolsOfTaskId))
                {
                    ModelState.AddModelError(string.Empty, "Invalid request data.");
                    return Page();
                }
                if (!string.IsNullOrWhiteSpace(NoteInf) && (Quantity == null || Quantity <= 0))
                {
                    TempData["ErrorMessage"] = "Quantity must be greater than 0 when Note is provided.";
                    return RedirectToPage("/FarmToolsOfTask/Index");
                }
                if (Quantity > 0 && string.IsNullOrWhiteSpace(NoteInf))
                {
                    TempData["ErrorMessage"] = "Note is required when Quantity is provided.";
                    return RedirectToPage("/FarmToolsOfTask/Index");
                }

                var apiUrl = $"https://localhost:7207/api/FarmToolsOfTasks/update-farm-tools-of-task-status-finish?FarmToolsOfTaskId={FarmToolsOfTaskRequest.FarmToolsOfTaskId}&NoteInf={NoteInf}&Quantity={Quantity}";

                var response = await _httpClient.PutAsync(apiUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToPage("/FarmToolsOfTask/Index");
                }

                TempData["SuccessMessage"] = "Farm tool task marked as finished successfully.";
                return RedirectToPage("/FarmToolsOfTask/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/FarmToolsOfTask/Index");
            }
        }

    }
}

