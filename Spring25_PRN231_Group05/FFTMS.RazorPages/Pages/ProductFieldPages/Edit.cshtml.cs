using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _configHttpClient;
        private readonly IConfiguration _configuration;

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5281/");
            
            _configuration = configuration;
            _configHttpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"])
            };
        }

        [BindProperty]
        public ProductFieldResponse ProductField { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Get ProductField using direct endpoint
                var response = await _httpClient.GetAsync($"odata/ProductField/{id}?$expand=Product,Field");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không thể truy xuất kế hoạch.";
                    return Page();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                ProductField = JsonSerializer.Deserialize<ProductFieldResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (ProductField == null)
                {
                    return NotFound();
                }

                await LoadDropdownData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    await LoadDropdownData();
            //    return Page();
            //}

            try
            {
                // Get original product field to check status
                var originalProductField = await GetOriginalProductField(ProductField.ProductFieldId);
                if (originalProductField == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy kế hoạch.");
                    await LoadDropdownData();
                    return Page();
                }

                // Validate EndDate > StartDate
                if (ProductField.EndDate <= ProductField.StartDate)
                {
                    ModelState.AddModelError(string.Empty, "Ngày kết thúc phải lớn hơn Ngày bắt đầu.");
                    await LoadDropdownData();
                    return Page();
                }

                // Check if trying to update Productivity or ProductivityUnit
               
                //if ((ProductField.Productivity != originalProductField.Productivity ||
                //     ProductField.ProductivityUnit != originalProductField.ProductivityUnit) &&
                //    ProductField.ProductFieldStatus != ProductFieldStatus.HARVESTED)
                //{
                //    ModelState.AddModelError(string.Empty,
                //        "Năng suất và Đơn vị năng suất chỉ có thể được cập nhật khi trạng thái là ĐÃ THU HOẠCH");
                //    await LoadDropdownData();
                //    return Page();
                //}

                var updateData = new
                {
                    productivity = ProductField.Productivity,
                    productivityUnit = ProductField.ProductivityUnit ?? "null",
                    startDate = ProductField.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    endDate = ProductField.EndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    productFieldStatus = (int)ProductField.ProductFieldStatus,
                    productId = ProductField.Product?.ProductId.ToString() ?? "",
                    fieldId = ProductField.Field?.FieldId.ToString() ?? ""
                };

                var apiUrl = $"odata/ProductField/{ProductField.ProductFieldId}";
                var updateResponse = await _httpClient.PutAsJsonAsync(apiUrl, updateData);

                if (updateResponse.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }

                var error = await updateResponse.Content.ReadAsStringAsync();
                try
                {
                    var errorDetails = JsonSerializer.Deserialize<ErrorResponse>(error, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ModelState.AddModelError(string.Empty, errorDetails.Message ?? error);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                
                await LoadDropdownData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                await LoadDropdownData();
                return Page();
            }
        }

        private async Task<ProductFieldResponse> GetOriginalProductField(Guid id)
        {
            // Using direct endpoint
            var response = await _httpClient.GetAsync($"odata/ProductField/{id}?$expand=Product,Field");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductFieldResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Using configuration endpoint for Products and Fields
                var productResponse = await _configHttpClient.GetAsync("odata/Product/get-all-product");
                if (productResponse.IsSuccessStatusCode)
                {
                    var products = await productResponse.Content.ReadFromJsonAsync<List<ProductDTO>>();
                    ViewData["ProductId"] = new SelectList(products, "ProductId", "ProductName", ProductField?.Product?.ProductId);
                }

                var fieldResponse = await _configHttpClient.GetAsync("odata/Field/get-all-field");
                if (fieldResponse.IsSuccessStatusCode)
                {
                    var fields = await fieldResponse.Content.ReadFromJsonAsync<List<FieldDTO>>();
                    ViewData["FieldId"] = new SelectList(fields, "FieldId", "FieldName", ProductField?.Field?.FieldId);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khi tải dữ liệu thả xuống: {ex.Message}");
            }
        }

        private class ErrorResponse
        {
            public string Message { get; set; }
        }
    }
}