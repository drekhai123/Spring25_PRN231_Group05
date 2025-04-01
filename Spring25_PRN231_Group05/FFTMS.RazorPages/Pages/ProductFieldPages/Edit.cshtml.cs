using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        [BindProperty]
        public ProductField ProductField { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Lấy ProductField từ API
                var response = await _httpClient.GetAsync($"odata/ProductField/{id}");
                if (response.IsSuccessStatusCode)
                {
                    ProductField = await response.Content.ReadFromJsonAsync<ProductField>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (ProductField == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }

                // Lấy danh sách Product để điền vào dropdown
                var productResponse = await _httpClient.GetAsync("odata/Product/get-all-product");
                if (productResponse.IsSuccessStatusCode)
                {
                    var products = await productResponse.Content.ReadFromJsonAsync<List<Product>>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewData["ProductId"] = new SelectList(products, "ProductId", "Description", ProductField.ProductId);
                }

                // Lấy danh sách Field để điền vào dropdown
                var fieldResponse = await _httpClient.GetAsync("odata/Field/get-all-field");
                if (fieldResponse.IsSuccessStatusCode)
                {
                    var fields = await fieldResponse.Content.ReadFromJsonAsync<List<FieldDTO>>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewData["FieldId"] = new SelectList(fields, "FieldId", "FieldName", ProductField.FieldId);
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ProductField: {ex.Message}");
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Nếu validation thất bại, cần tải lại danh sách Product và Field cho dropdown
                await LoadDropdownData();
                return Page();
            }

            try
            {
                // Cập nhật ProductField qua API
                var response = await _httpClient.PutAsJsonAsync($"odata/ProductField/{ProductField.ProductFieldId}", ProductField);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update ProductField. Please try again.");
                    await LoadDropdownData();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ProductField: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the ProductField.");
                await LoadDropdownData();
                return Page();
            }
        }

        private async Task LoadDropdownData()
        {
            // Tải lại danh sách Product
            var productResponse = await _httpClient.GetAsync("odata/Product/get-all-product");
            if (productResponse.IsSuccessStatusCode)
            {
                var products = await productResponse.Content.ReadFromJsonAsync<List<Product>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ViewData["ProductId"] = new SelectList(products, "ProductId", "Description", ProductField.ProductId);
            }

            // Tải lại danh sách Field
            var fieldResponse = await _httpClient.GetAsync("odata/Field/get-all-field");
            if (fieldResponse.IsSuccessStatusCode)
            {
                var fields = await fieldResponse.Content.ReadFromJsonAsync<List<FieldDTO>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ViewData["FieldId"] = new SelectList(fields, "FieldId", "FieldName", ProductField.FieldId);
            }
        }
    }
}