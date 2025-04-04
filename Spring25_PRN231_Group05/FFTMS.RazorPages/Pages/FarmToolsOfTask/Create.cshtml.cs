using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [BindProperty]
        public CreateFarmToolsOfTaskRequestDTO FarmToolsOfTask { get; set; } = new();

        public SelectList FarmToolsList { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string selectedToolsJson { get; set; }

        public List<FarmToolsResponseDTO> AvailableTools { get; set; } = new List<FarmToolsResponseDTO>();

        public async Task<IActionResult> OnGetAsync(string userTaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(userTaskId))
                {
                    return BadRequest("UserTaskId is required.");
                }

                FarmToolsOfTask = new CreateFarmToolsOfTaskRequestDTO
                {
                    UserTaskId = userTaskId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                };

                await LoadFarmToolsList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Đảm bảo form data có giá trị
                if (FarmToolsOfTask == null || string.IsNullOrEmpty(FarmToolsOfTask.UserTaskId))
                {
                    ModelState.AddModelError("", "Thiếu thông tin tác vụ");
                    await LoadFarmToolsList();
                    return Page();
                }

                // Kiểm tra công cụ đã chọn
                if (string.IsNullOrEmpty(selectedToolsJson))
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất một công cụ trang trại");
                    await LoadFarmToolsList();
                    return Page();
                }

                // Thêm kiểm tra số lượng tools
                if (!string.IsNullOrEmpty(selectedToolsJson))
                {
                    var selectedTools = JsonSerializer.Deserialize<List<FarmToolQuantityDTO>>(
                        selectedToolsJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    var toolsDataJson = ViewData["ToolsData"]?.ToString();
                    if (!string.IsNullOrEmpty(toolsDataJson))
                    {
                        var availableTools = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(
                            toolsDataJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                        foreach (var selectedTool in selectedTools)
                        {
                            var availableTool = availableTools.FirstOrDefault(t => t.FarmToolsId == selectedTool.FarmToolsId);
                            if (availableTool != null && selectedTool.Quantity > availableTool.FarmToolsQuantity)
                            {
                                ModelState.AddModelError("", $"Số lượng của {availableTool.FarmToolsName} vượt quá số lượng có sẵn ({availableTool.FarmToolsQuantity})");
                                await LoadFarmToolsList();
                                return Page();
                            }
                        }
                    }
                }

                // Deserialize danh sách công cụ
                List<FarmToolQuantityDTO> toolsList;
                try
                {
                    toolsList = JsonSerializer.Deserialize<List<FarmToolQuantityDTO>>(
                        selectedToolsJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (toolsList == null || !toolsList.Any())
                    {
                        ModelState.AddModelError("", "Không có công cụ trang trại hợp lệ nào được chọn");
                        await LoadFarmToolsList();
                        return Page();
                    }
                }
                catch (JsonException ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi xử lý dữ liệu công cụ: {ex.Message}");
                    await LoadFarmToolsList();
                    return Page();
                }

                // Chuẩn bị request data
                var requestData = new
                {
                    startDate = FarmToolsOfTask.StartDate,
                    endDate = FarmToolsOfTask.EndDate,
                    farmToolOfTaskQuantity = 0,
                    farmToolOfTaskUnit = "unit",
                    createDate = DateTime.UtcNow,
                    updateDate = DateTime.UtcNow,
                    farmToolsId = "",
                    userTaskId = FarmToolsOfTask.UserTaskId,
                    listFarmTools = toolsList
                };

                // Serialize request data
                string requestJson = JsonSerializer.Serialize(requestData);

                // Sử dụng HttpClient trực tiếp
                using (var httpClient = new HttpClient())
                {
                    // Lấy token nếu cần
                    var token = Request.Cookies["AuthToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    // Tạo content và gọi API
                    var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    var apiUrl = "https://localhost:7207/api/FarmToolsOfTasks/create-farm-tools-of-task";
                    var response = await httpClient.PostAsync(apiUrl, stringContent);

                    // Trả về kết quả
                    if (response.IsSuccessStatusCode)
                    {
                        // Sau khi thêm farm tools thành công, cập nhật status của UserTask lên 1 (Processing)
                        try
                        {
                            // Tạo request cập nhật status của UserTask
                            var updateTaskRequest = new
                            {
                                status = 1, // Processing
                            };

                            var updateTaskJson = JsonSerializer.Serialize(updateTaskRequest);
                            var updateTaskContent = new StringContent(updateTaskJson, Encoding.UTF8, "application/json");

                            // Gọi API cập nhật UserTask - sửa từ api sang odata
                            var updateTaskUrl = $"https://localhost:7207/odata/UserTask/update-status/{FarmToolsOfTask.UserTaskId}";
                            var updateTaskResponse = await httpClient.PutAsync(updateTaskUrl, updateTaskContent);

                            if (!updateTaskResponse.IsSuccessStatusCode)
                            {
                                // Đọc nội dung lỗi để debug
                                var errorContent = await updateTaskResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"Failed to update task status: {updateTaskResponse.StatusCode}, Error: {errorContent}");

                                // Vẫn tiếp tục quy trình
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating task status: {ex.Message}");
                            // Vẫn tiếp tục quy trình
                        }

                        TempData["SuccessMessage"] = "Đã thêm công cụ trang trại thành công";
                        return RedirectToPage("/UserTasks/Index");
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Failed to add farm tools: {response.StatusCode}");
                        await LoadFarmToolsList();
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                await LoadFarmToolsList();
                return Page();
            }
        }

        private async Task LoadFarmToolsList()
        {
            var response = await _httpClient.GetAsync("https://localhost:7207/odata/FarmTools/get-all-farm-tools?$filter=Status eq true");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tools = JsonSerializer.Deserialize<List<FarmToolsResponseDTO>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Chỉ lấy các tool có số lượng > 0
                AvailableTools = tools?.Where(t => t.FarmToolsQuantity > 0).ToList() ?? new List<FarmToolsResponseDTO>();

                // Lưu danh sách tools để sử dụng trong OnPost
                ViewData["ToolsData"] = JsonSerializer.Serialize(AvailableTools);

                // Tạo SelectList từ danh sách tools có sẵn
                FarmToolsList = new SelectList(AvailableTools, "FarmToolsId", "FarmToolsName");
            }
            else
            {
                AvailableTools = new List<FarmToolsResponseDTO>();
                FarmToolsList = new SelectList(new List<FarmToolsResponseDTO>(), "FarmToolsId", "FarmToolsName");
                ModelState.AddModelError("", "Lỗi khi tải danh sách công cụ trang trại.");
            }
        }
    }
}