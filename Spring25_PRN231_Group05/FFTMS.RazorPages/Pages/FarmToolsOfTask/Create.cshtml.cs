using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FFTMS.RazorPages.Pages.FarmToolsOfTask
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGetAsync(string userTaskId)
        {
            if (string.IsNullOrEmpty(userTaskId))
            {
                return BadRequest("UserTaskId is required.");
            }

            FarmToolsOfTask = new CreateFarmToolsOfTaskRequestDTO
            {
                UserTaskId = userTaskId
            };

            ViewData["FarmToolsId"] = new SelectList(await GetFarmToolsAsync(), "FarmToolsId", "FarmToolsName");
            return Page();
        }

        [BindProperty]
        public CreateFarmToolsOfTaskRequestDTO FarmToolsOfTask { get; set; } = new();

        [BindProperty]
        public List<FarmToolQuantityDTO> SelectedFarmTools { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SelectedFarmTools == null || !SelectedFarmTools.Any())
            {
                ModelState.AddModelError("", "Please select at least one farm tool with a valid quantity.");
                return Page();
            }

            FarmToolsOfTask.ListFarmTools = SelectedFarmTools;
            FarmToolsOfTask.CreateDate = DateTime.UtcNow;
            FarmToolsOfTask.UpdateDate = DateTime.UtcNow;

            var jsonContent = new StringContent(JsonSerializer.Serialize(FarmToolsOfTask), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7207/odata/FarmToolsOfTasks/create-farm-tools-of-task", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create farm tools task.");
                return Page();
            }
        }

        private async Task<List<FarmToolsResponseDTO>> GetFarmToolsAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=Status eq true");
            return response.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<FarmToolsResponseDTO>();
        }
    }
}
