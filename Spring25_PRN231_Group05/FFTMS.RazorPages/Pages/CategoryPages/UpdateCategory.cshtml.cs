using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.CategoryPages
{
    public class UpdateCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public UpdateCategoryModel(HttpClient httpClient, IWebHostEnvironment environment)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7207/");
            _environment = environment;
        }

        [BindProperty]
        public UpdateCategoryInputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class UpdateCategoryInputModel
        {
            public Guid CategoryId { get; set; }

            [Required(ErrorMessage = "Category name is required")]
            [StringLength(30, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 30 characters")]
            [Display(Name = "Category Name")]
            public string CategoryName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Description is required")]
            [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
            public string Description { get; set; } = string.Empty;

            [Display(Name = "Category Image")]
            public IFormFile? ImageFile { get; set; }

            public string? ExistingImageUrl { get; set; }

            [Required(ErrorMessage = "Status is required")]
            public bool Status { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"odata/Category/get-categories-by-id?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Category with ID {id} not found.";
                    return Page();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<CategoryResponseDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (category == null)
                {
                    return NotFound();
                }

                Input = new UpdateCategoryInputModel
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ExistingImageUrl = category.CategoryImageUrl,
                    Status = category.Status
                };

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while fetching data: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string imageUrl = Input.ExistingImageUrl ?? "";

                if (Input.ImageFile != null && Input.ImageFile.Length > 0)
                {
                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                    if (!allowedTypes.Contains(Input.ImageFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("Input.ImageFile", "Only .jpg, .jpeg and .png files are allowed");
                        return Page();
                    }

                    // Validate file size (max 5MB)
                    if (Input.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Input.ImageFile", "File size cannot exceed 5MB");
                        return Page();
                    }

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{Input.ImageFile.FileName}";
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "categories");
                    Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Input.ImageFile.CopyToAsync(fileStream);
                    }

                    imageUrl = $"/uploads/categories/{fileName}";
                }

                var updateData = new
                {
                    categoryName = Input.CategoryName,
                    description = Input.Description,
                    categoryImageUrl = imageUrl,
                    status = Input.Status
                };

                var apiUrl = $"odata/Category/update-category?id={Input.CategoryId}";
                var response = await _httpClient.PutAsJsonAsync(apiUrl, updateData);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./ListCategory");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating category: {error}");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}
