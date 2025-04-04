using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using System.Net.Http.Json;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHubContext<ProductFieldHub> _hubContext;

        public DeleteModel(HttpClient httpClient, IHubContext<ProductFieldHub> hubContext)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5281/");
            _hubContext = hubContext;
        }

        [BindProperty]
        public ProductField ProductField { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiUrl = $"https://localhost:5281/odata/ProductFields/get-by/{id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var response = await _httpClient.GetAsync($"odata/ProductField/{id}?$expand=Product,Field");
                
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Failed to retrieve the Product Field. It may have been deleted or you may not have permission to view it.";
                    return Page();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                ProductField = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (ProductField == null)
                {
                    return NotFound();
                }

                return Page();
            }
            else
            {
                ErrorMessage = $"An error occurred while retrieving the Product Field: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"odata/ProductField/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    await _hubContext.Clients.All.SendAsync("ProductFieldDeleted", id);
                    return RedirectToPage("./Index");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ErrorMessage = errorResponse?.Message ?? "Failed to delete the Product Field.";
                }
                catch
                {
                    ErrorMessage = $"Failed to delete the Product Field. Status code: {response.StatusCode}";
                }
                
                // Reload the product field data for the view
                await OnGetAsync(id);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while deleting the Product Field: {ex.Message}";
                // Reload the product field data for the view
                await OnGetAsync(id);
                return Page();
            }
        }

        private class ErrorResponse
        {
            public string Message { get; set; }
        }
    }
}

            var apiUrl = $"https://localhost:5281/odata/ProductFields/delete-by-{id}";
            var response = await _httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ErrorMessage = "Failed to delete the product field.";
                return Page();
            }
        }
    }
}