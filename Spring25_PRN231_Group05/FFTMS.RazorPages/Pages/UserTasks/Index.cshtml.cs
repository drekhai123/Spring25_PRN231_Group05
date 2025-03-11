using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;
using System.Text.Json.Serialization;
using System.Text;

namespace FFTMS.RazorPages.Pages.UserTasks
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            UserTasks = new List<UserTaskResponseDTO>();
            FarmTools = new List<FarmToolsResponseDTO>();
        }

        public IList<UserTaskResponseDTO> UserTasks { get; set; }
        public IList<FarmToolsResponseDTO> FarmTools { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Auth/LoginPage");
                }

                var userId = JwtHelper.GetUserIdFromToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var userTaskResponse = await _httpClient.GetAsync($"https://localhost:7207/odata/UserTask?$filter=UserId eq {userId}");
                var farmToolsResponse = await _httpClient.GetAsync("https://localhost:7207/odata/FarmTools/get-all-farm-tools");

                if (userTaskResponse.IsSuccessStatusCode && farmToolsResponse.IsSuccessStatusCode)
                {
                    var jsonUserTasks = await userTaskResponse.Content.ReadAsStringAsync();
                    var jsonFarmTools = await farmToolsResponse.Content.ReadAsStringAsync();

                    UserTasks = JsonSerializer.Deserialize<List<UserTaskResponseDTO>>(jsonUserTasks, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UserTaskResponseDTO>();

                    FarmTools = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(jsonFarmTools, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<FarmToolsResponseDTO>();
                }
                else
                {
                    ErrorMessage = "Failed to load data from API";
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmTaskAsync(Guid id, string farmToolId)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Auth/LoginPage");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7207/odata/UserTask/{id}", new
                {
                    Status = 1, // Processing status
                    FarmToolIds = new[] { farmToolId }
                });

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to confirm task";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostCompleteTaskAsync(Guid id)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Auth/LoginPage");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                //// Lấy task hiện tại để giữ nguyên FarmToolIds
                //var currentTask = await _httpClient.GetAsync($"https://localhost:7207/odata/UserTask/{id}");
                //var taskContent = await currentTask.Content.ReadAsStringAsync();
                //var userTask = JsonSerializer.Deserialize<UserTaskResponseDTO>(taskContent, new JsonSerializerOptions
                //{
                //    PropertyNameCaseInsensitive = true
                //});

                //// Gửi request update với FarmToolIds hiện tại
                //var response = await _httpClient.PutAsJsonAsync($"https://localhost:7207/odata/UserTask/{id}", new
                //{
                //    Status = 2,  // Completed
                //    FarmToolIds = userTask?.FarmTools?.Select(f => f.FarmToolsId)?.ToArray() ?? Array.Empty<string>()
                //});

                //if (!response.IsSuccessStatusCode)
                //{
                //    TempData["ErrorMessage"] = "Failed to complete task";
                //}

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostCompleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Task ID is required");

                using (var httpClient = new HttpClient())
                {
                    // Lấy token nếu cần
                    var token = Request.Cookies["AuthToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    // Tạo request cập nhật status của UserTask thành 2 (Completed)
                    var updateTaskRequest = new
                    {
                        status = 2 // Completed
                    };

                    var updateTaskJson = JsonSerializer.Serialize(updateTaskRequest);
                    var updateTaskContent = new StringContent(updateTaskJson, Encoding.UTF8, "application/json");

                    // Gọi API cập nhật UserTask
                    var updateTaskUrl = $"https://localhost:7207/odata/UserTask/update-status/{id}";
                    var response = await httpClient.PutAsync(updateTaskUrl, updateTaskContent);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Task marked as completed successfully";
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Failed to complete task: {response.StatusCode}";
                        // Log lỗi nếu cần
                        Console.WriteLine($"Error completing task: {errorContent}");
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}