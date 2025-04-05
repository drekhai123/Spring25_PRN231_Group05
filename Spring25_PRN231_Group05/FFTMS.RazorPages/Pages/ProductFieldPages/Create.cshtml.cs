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
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _configHttpClient;
        private readonly IConfiguration _configuration;

        public CreateModel(HttpClient httpClient, IConfiguration configuration)
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
        public ProductField ProductField { get; set; } = new ProductField();

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadDropdownData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return Page();
            }

            try
            {
                if (ProductField.StartDate < DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "Ngày bắt đầu phải bằng hoặc lớn hơn bây giờ");
                    await LoadDropdownData();
                    return Page();
                }
                // Validate EndDate > StartDate
                if (ProductField.EndDate <= ProductField.StartDate)
                {
                    ModelState.AddModelError(string.Empty, "Ngày kết thúc phải lớn hơn ngày bắt đầu");
                    await LoadDropdownData();
                    return Page();
                }

                // Set initial status to GROWING
                ProductField.ProductFieldStatus = ProductFieldStatus.READYTOPLANT;

                // Set create and update dates
                ProductField.CreateDate = DateTime.Now;
                ProductField.UpdateDate = DateTime.Now;

                var response = await _httpClient.PostAsJsonAsync("odata/ProductField", ProductField);
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating product field: {ex.Message}");
                await LoadDropdownData();
                return Page();
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Load Products using configuration endpoint
                var productResponse = await _configHttpClient.GetAsync("odata/Product/get-all-product");
                if (productResponse.IsSuccessStatusCode)
                {
                    var products = await productResponse.Content.ReadFromJsonAsync<List<ProductDTO>>();
                    ViewData["ProductId"] = new SelectList(products, "ProductId", "ProductName");
                }

                // Load Fields using configuration endpoint
                var fieldResponse = await _configHttpClient.GetAsync("odata/Field/get-all-field");
                if (fieldResponse.IsSuccessStatusCode)
                {
                    var fields = await fieldResponse.Content.ReadFromJsonAsync<List<FieldDTO>>();
                    ViewData["FieldId"] = new SelectList(fields, "FieldId", "FieldName");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error loading dropdown data: {ex.Message}");
            }
        }

        private class ErrorResponse
        {
            public string Message { get; set; }
        }
    }
}