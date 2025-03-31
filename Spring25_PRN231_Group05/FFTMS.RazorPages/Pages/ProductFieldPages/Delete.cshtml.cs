//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using FlowerFarmTaskManagementSystem.BusinessObject.Models;
//using FlowerFarmTaskManagementSystem.DataAccess;
//using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
//using System.Text.Json;
//using System.Net.Http;
//using Azure;
//using Microsoft.AspNetCore.SignalR;
//using FlowerFarmTaskManagementSystem.BusinessLogic.Service;

//namespace FFTMS.RazorPages.Pages.ProductFieldPages
//{
//    public class DeleteModel : PageModel
//    {
//        private readonly HttpClient _httpClient;
//        private readonly IHubContext<ProductFieldHub> _hubContext;
//        public DeleteModel(HttpClient httpClient, IHubContext<ProductFieldHub> hubContext)
//        {
//            _httpClient = httpClient;
//            _hubContext = hubContext;
//        }

//        [BindProperty]
//        public ProductField ProductField { get; set; } = default!;
//        public string ErrorMessage { get; set; }
//        public async Task<IActionResult> OnGetAsync(Guid? id)
//        {
//            if (id != null)
//            {
//                return Page();
//            }

//            try
//            {
//                var apiUrl = $"http://localhost:5281/odata/ProductFields/get-by-{id}";
//                ;
//                var response = await _httpClient.GetAsync(apiUrl);

//                if (!response.IsSuccessStatusCode)
//                {

//                    return NotFound();
//                }

//                var jsonResponse = await response.Content.ReadAsStringAsync();
//                var parsedResponse = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });
//                ProductField = parsedResponse;
//            }
//            catch (Exception ex)
//            {
//                ViewData["ErrorMessage"] = $"An error occurred: {ex.Message}";
//                return Page();
//            }

//            return Page();
//        }

//        public async Task<IActionResult> OnPostAsync(Guid? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            try
//            {
//                var apiUrl = $"http://localhost:5281/odata/ProductFields/delete-by-id/{id}";
//                var response = await _httpClient.DeleteAsync(apiUrl);

//                if (response.IsSuccessStatusCode)
//                {
//                    await _hubContext.Clients.All.SendAsync("ProductFieldDeleted", id);
//                    return RedirectToPage("/Index");
//                }
//                else
//                {
//                    ErrorMessage = $"Error deleting product field. Status code: {response.StatusCode}";
//                    return Page();
//                }
//            }
//            catch (Exception ex)
//            {
//                ErrorMessage = $"Error deleting product field: {ex.Message}";
//                return Page();
//            }
//        }
//    }
//    }



using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public ProductFieldResponse ProductField { get; set; }

        // Define ErrorMessage property with a default value
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                ErrorMessage = "Product Field ID is required.";
                return Page();
            }

            var apiUrl = $"https://localhost:5281/odata/ProductFields/get-by/{id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                ProductField = JsonSerializer.Deserialize<ProductFieldResponse>(jsonResponse,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            else
            {
                ErrorMessage = "Failed to load the product field.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                ErrorMessage = "Product Field ID is required.";
                return Page();
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