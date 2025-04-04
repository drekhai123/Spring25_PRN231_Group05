using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
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
        public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ProductFieldList { get; set; } = new List<SelectListItem>();
        public string? ErrorMessage { get; set; }
        public List<ProductFieldResponse> ProductFieldsData { get; set; } = new List<ProductFieldResponse>();

        public async Task<IActionResult> OnGetAsync(Guid? productFieldId = null, bool isHarvesting = false)
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

                // Fetch users for staff assignment dropdown
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (userResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    UserList = users?
                        .Where(u => u.IsActive && u.Role == "Staff")
                        .Select(u => new SelectListItem
                        {
                            Value = u.UserId.ToString(),
                            Text = u.UserName
                        })
                        .ToList() ?? new List<SelectListItem>();
                }

                // Fetch product fields for dropdown
                var productFieldResponse = await _httpClient.GetAsync("http://localhost:5281/odata/ProductField");
                if (productFieldResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await productFieldResponse.Content.ReadAsStringAsync();
                    var productFields = JsonSerializer.Deserialize<List<ProductFieldResponse>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // If creating a harvest task, only show fields that are ready to harvest
                    if (isHarvesting)
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOHARVEST)
                            .ToList();
                    }
                    else
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOPLANT)
                            .ToList();
                    }

                    ProductFieldList = productFields?
                        .Select(pf => new SelectListItem
                        {
                            Value = pf.ProductFieldId.ToString(),
                            Text = $"{pf.Product?.ProductName} ({pf.Field?.FieldName})"
                        })
                        .ToList() ?? new List<SelectListItem>();

                    ProductFieldsData = productFields ?? new List<ProductFieldResponse>();

                    // If productFieldId is provided, pre-select it
                    if (productFieldId.HasValue)
                    {
                        var selectedField = ProductFieldList.FirstOrDefault(pf => pf.Value == productFieldId.ToString());
                        if (selectedField != null)
                        {
                            selectedField.Selected = true;
                            Task.ProductFieldId = productFieldId.Value;

                            // Pre-fill job title for harvest task
                            if (isHarvesting)
                            {
                                Task.JobTitle = "Thu hoạch và phân loại hoa";
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading page: {ex.Message}";
                return Page();
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

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7207/odata/Task", Task);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = responseContent ?? "Error creating task";
                
                // Reload dropdown data when validation fails
                // Fetch users for staff assignment dropdown
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (userResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    UserList = users?
                        .Where(u => u.IsActive && u.Role == "Staff")
                        .Select(u => new SelectListItem
                        {
                            Value = u.UserId.ToString(),
                            Text = u.UserName
                        })
                        .ToList() ?? new List<SelectListItem>();
                }

                // Fetch product fields for dropdown
                var productFieldResponse = await _httpClient.GetAsync("http://localhost:5281/odata/ProductField");
                if (productFieldResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await productFieldResponse.Content.ReadAsStringAsync();
                    var productFields = JsonSerializer.Deserialize<List<ProductFieldResponse>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Filter based on job title (if it's a harvest task)
                    bool isHarvesting = Task.JobTitle?.Contains("Thu hoạch", StringComparison.OrdinalIgnoreCase) ?? false;
                    if (isHarvesting)
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOHARVEST)
                            .ToList();
                    }
                    else
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOPLANT)
                            .ToList();
                    }

                    ProductFieldList = productFields?
                        .Select(pf => new SelectListItem
                        {
                            Value = pf.ProductFieldId.ToString(),
                            Text = $"{pf.Product?.ProductName} ({pf.Field?.FieldName})",
                            Selected = pf.ProductFieldId == Task.ProductFieldId
                        })
                        .ToList() ?? new List<SelectListItem>();

                    ProductFieldsData = productFields ?? new List<ProductFieldResponse>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                // Also reload dropdown data in case of exception
                await LoadDropdownData();
                return Page();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Fetch users for staff assignment dropdown
                var userResponse = await _httpClient.GetAsync("https://localhost:7207/odata/User");
                if (userResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await userResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    UserList = users?
                        .Where(u => u.IsActive && u.Role == "Staff")
                        .Select(u => new SelectListItem
                        {
                            Value = u.UserId.ToString(),
                            Text = u.UserName
                        })
                        .ToList() ?? new List<SelectListItem>();
                }

                // Fetch product fields for dropdown
                var productFieldResponse = await _httpClient.GetAsync("http://localhost:5281/odata/ProductField");
                if (productFieldResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await productFieldResponse.Content.ReadAsStringAsync();
                    var productFields = JsonSerializer.Deserialize<List<ProductFieldResponse>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    bool isHarvesting = Task.JobTitle?.Contains("Thu hoạch", StringComparison.OrdinalIgnoreCase) ?? false;
                    if (isHarvesting)
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOHARVEST)
                            .ToList();
                    }
                    else
                    {
                        productFields = productFields?
                            .Where(pf => pf.ProductFieldStatus == ProductFieldStatus.READYTOPLANT)
                            .ToList();
                    }

                    ProductFieldList = productFields?
                        .Select(pf => new SelectListItem
                        {
                            Value = pf.ProductFieldId.ToString(),
                            Text = $"{pf.Product?.ProductName} ({pf.Field?.FieldName})",
                            Selected = pf.ProductFieldId == Task.ProductFieldId
                        })
                        .ToList() ?? new List<SelectListItem>();

                    ProductFieldsData = productFields ?? new List<ProductFieldResponse>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dropdown data: {ex.Message}";
            }
        }
    }
}