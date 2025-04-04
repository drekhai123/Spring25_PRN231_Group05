using FFTMS.RazorPages.Helpers;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IEnumerable<TaskResponseDTO> Tasks { get; set; } = new List<TaskResponseDTO>();
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "StartDate"; // Default sort by start date

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "desc"; // Default sort order

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

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

                var response = await _httpClient.GetAsync("https://localhost:7207/odata/Task");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var allTasks = JsonSerializer.Deserialize<List<TaskResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Apply search filter if search term exists
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        allTasks = allTasks.Where(t =>
                            t.JobTitle.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                            t.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (t.Product?.ProductName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (t.Field?.FieldName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                        ).ToList();
                    }

                    // Apply sorting
                    allTasks = SortColumn?.ToLower() switch
                    {
                        "jobtitle" => SortOrder == "asc" 
                            ? allTasks.OrderBy(t => t.JobTitle).ToList()
                            : allTasks.OrderByDescending(t => t.JobTitle).ToList(),
                        
                        "description" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.Description).ToList()
                            : allTasks.OrderByDescending(t => t.Description).ToList(),
                        
                        "productfield" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.Product?.ProductName).ThenBy(t => t.Field?.FieldName).ToList()
                            : allTasks.OrderByDescending(t => t.Product?.ProductName).ThenByDescending(t => t.Field?.FieldName).ToList(),
                        
                        "fieldstatus" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.ProductFieldStatus).ToList()
                            : allTasks.OrderByDescending(t => t.ProductFieldStatus).ToList(),
                        
                        "startdate" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.StartDate).ToList()
                            : allTasks.OrderByDescending(t => t.StartDate).ToList(),
                        
                        "enddate" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.EndDate).ToList()
                            : allTasks.OrderByDescending(t => t.EndDate).ToList(),
                        
                        "taskstatus" => SortOrder == "asc"
                            ? allTasks.OrderBy(t => t.TaskStatus).ToList()
                            : allTasks.OrderByDescending(t => t.TaskStatus).ToList(),
                        
                        _ => allTasks.OrderByDescending(t => t.StartDate).ToList() // Default sort
                    };

                    // Store total items before pagination
                    TotalItems = allTasks.Count;

                    // Apply pagination
                    Tasks = allTasks
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    return Page();
                }

                ErrorMessage = "Error loading tasks. Please try again later.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}