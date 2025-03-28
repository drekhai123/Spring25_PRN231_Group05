using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using FFTMS.RazorPages.Helpers;
using System.Text.Json.Serialization;
using System.Text;
using NuGet.Packaging;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;

namespace FFTMS.RazorPages.Pages.UserTasks
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            UserTasks = new List<UserTaskFarmToolsResponseDTO>();
            FarmTools = new List<FarmToolsResponseDTO>();
            FarmToolsOfTask = new List<FarmToolsOfTaskResponseDTO>();

        }

        public IList<UserTaskFarmToolsResponseDTO> UserTasks { get; set; }
        public IList<FarmToolsResponseDTO> FarmTools { get; set; }
        public IList<FarmToolsOfTaskResponseDTO> FarmToolsOfTask { get; set; }

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

                // 1. Lấy danh sách UserTasks của User
                var userTaskResponse = await _httpClient.GetAsync($"https://localhost:7207/odata/UserTask?$filter=UserId eq {userId}");

                if (!userTaskResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = "Failed to load user tasks from API";
                    return Page();
                }

                var jsonUserTasks = await userTaskResponse.Content.ReadAsStringAsync();
                UserTasks = JsonSerializer.Deserialize<List<UserTaskFarmToolsResponseDTO>>(jsonUserTasks, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<UserTaskFarmToolsResponseDTO>();

                // Filter out UserTasks associated with inactive TaskWorks
                UserTasks = UserTasks.Where(task => task.Task != null && task.Task.Status).ToList();

                // Sau khi lấy UserTasks, in thông tin để debug
                foreach (var task in UserTasks)
                {
                    Console.WriteLine($"Task ID: {task.UserTaskId}, Status: {task.Status}, ImageUrl: {task.ImageUrl}");
                }

                // 2. Lấy danh sách FarmToolOfTask bằng vòng lặp for
                var farmToolIds = new HashSet<Guid>(); // Dùng HashSet để tránh trùng lặp

                for (int i = 0; i < UserTasks.Count; i++)
                {
                    var userTaskId = UserTasks[i].UserTaskId;
                    var farmToolOfTaskResponse = await _httpClient.GetAsync($"https://localhost:7207/api/FarmToolsOfTasks/get-all-farm-tools-of-task?$filter=UserTaskId eq '{userTaskId}'");

                    if (farmToolOfTaskResponse.IsSuccessStatusCode)
                    {
                        var jsonFarmToolOfTask = await farmToolOfTaskResponse.Content.ReadAsStringAsync();
                        var farmToolOfTaskList = JsonSerializer.Deserialize<List<FarmToolsOfTaskResponseDTO>>(jsonFarmToolOfTask, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<FarmToolsOfTaskResponseDTO>();

                        var farmToolOfTaskDict = farmToolOfTaskList.GroupBy(f => f.UserTaskId)
                            .ToDictionary(g => g.Key, g => g.ToList());

                        foreach (var task in UserTasks)
                        {
                            var USTID = task.UserTaskId.ToString();
                            if (farmToolOfTaskDict.TryGetValue(USTID, out var tools))
                            {
                                task.FarmToolsOfTask = tools;
                            }
                        }
                    }
                }

                    // 3. Lấy danh sách FarmTools dựa vào FarmToolIds đã thu thập được
                    var farmToolIdList = farmToolIds.ToList();
                FarmTools = new List<FarmToolsResponseDTO>();

                for (int i = 0; i < farmToolIdList.Count; i++)
                {
                    var farmToolId = farmToolIdList[i];
                    var farmToolResponse = await _httpClient.GetAsync($"https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=FarmToolsId eq '{farmToolId}'");

                    if (farmToolResponse.IsSuccessStatusCode)
                    {
                        var jsonFarmTool = await farmToolResponse.Content.ReadAsStringAsync();
                        var farmTool = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(jsonFarmTool, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<FarmToolsResponseDTO>();

                        FarmTools.AddRange(farmTool);
                    }
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

        public async Task<IActionResult> OnPostCompleteAsync(string id, string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Task ID is required");

                // Kiểm tra imageUrl có được cung cấp không
                if (string.IsNullOrEmpty(imageUrl))
                {
                    TempData["ErrorMessage"] = "You must upload an image before completing the task";
                    return RedirectToPage();
                }

                using (var httpClient = new HttpClient())
                {
                    // Lấy token nếu cần
                    var token = Request.Cookies["AuthToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    // Tạo request cập nhật status và imageUrl của UserTask
                    var updateTaskRequest = new
                    {
                        status = 2, // Completed
                        imageUrl = imageUrl // Thêm ImageUrl vào request
                    };

                    var updateTaskJson = JsonSerializer.Serialize(updateTaskRequest);
                    var updateTaskContent = new StringContent(updateTaskJson, Encoding.UTF8, "application/json");

                    // Gọi API cập nhật UserTask
                    var updateTaskUrl = $"https://localhost:7207/odata/UserTask/update-status/{id}";
                    var response = await httpClient.PutAsync(updateTaskUrl, updateTaskContent);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Task has been marked as completed";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to complete task: {response.StatusCode}";
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}