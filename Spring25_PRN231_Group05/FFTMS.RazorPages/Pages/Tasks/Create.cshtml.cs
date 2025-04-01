using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Security.Claims;
using FFTMS.RazorPages.Helpers;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TaskRequestDTO Task { get; set; } = new TaskRequestDTO();
        public SelectList UserList { get; set; }
        public SelectList ProductFieldList { get; set; }
        public string ErrorMessage { get; set; }
        public List<ProductFieldRequest> ProductFieldsData { get; set; }

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
                if (role != "Manager")
                {
                    return RedirectToPage("/Index");
                }

                var userId = JwtHelper.GetUserIdFromToken(token);
                Task.AssignedBy = userId;

                await LoadUserList();
                await LoadProductFieldList();

                // Set default dates
                Task.StartDate = DateTime.Now;
                Task.EndDate = DateTime.Now.AddHours(2);

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        private async Task LoadUserList()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var usersList = users.Where(u => u.Role == "Staff")
                                      .Select(u => new SelectListItem
                                      {
                                          Value = u.UserId.ToString(),
                                          Text = $"{u.UserName} ({u.Email})"
                                      });

                    UserList = new SelectList(usersList, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading users: {ex.Message}";
            }
        }

        private async Task LoadProductFieldList()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5281/odata/ProductFields/get-all-productField");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var productFields = JsonSerializer.Deserialize<List<ProductFieldRequest>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ProductFieldsData = productFields;

                    var productFieldsList = productFields.Select(pf => new SelectListItem
                    {
                        Value = pf.ProductFieldId.ToString(),
                        Text = $"{pf.Product.ProductName} - {pf.ProductivityUnit}"
                    });

                    ProductFieldList = new SelectList(productFieldsList, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading product fields: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Auth/LoginPage");
                }

                if (string.IsNullOrEmpty(Task.AssignedBy))
                {
                    Task.AssignedBy = JwtHelper.GetUserIdFromToken(token);
                }
                Task.Status = true;
                Task.TaskStatus = FlowerFarmTaskManagementSystem.BusinessObject.Models.TaskProgressStatus.INPROGRESS;

                // Debug: Kiểm tra dữ liệu trước khi gửi
                Console.WriteLine($"StartDate: {Task.StartDate}");
                Console.WriteLine($"EndDate: {Task.EndDate}");
                Console.WriteLine($"ProductFieldId: {Task.ProductFieldId}");
                Console.WriteLine($"UserTasks count: {Task.UserTasks?.Count ?? 0}");

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7207/odata/Task", Task);
                var responseContent = await response.Content.ReadAsStringAsync();

                // In ra toàn bộ nội dung response để xem lỗi chi tiết
                Console.WriteLine($"API Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = responseContent ?? "Error creating task";
                await LoadUserList();
                await LoadProductFieldList();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await LoadUserList();
                await LoadProductFieldList();
                return Page();
            }
        }
    }
}