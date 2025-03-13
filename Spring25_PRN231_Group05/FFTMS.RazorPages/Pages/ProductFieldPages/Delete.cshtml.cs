using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System.Text.Json;
using System.Net.Http;
using Azure;
using Microsoft.AspNetCore.SignalR;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;

namespace FFTMS.RazorPages.Pages.ProductFieldPages
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHubContext<ProductFieldHub> _hubContext;
        public DeleteModel(HttpClient httpClient, IHubContext<ProductFieldHub> hubContext)
        {
            _httpClient = httpClient;
            _hubContext = hubContext;
        }

        [BindProperty]
        public ProductField ProductField { get; set; } = default!;
        public string ErrorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id != null)
            {
                return Page();
            }

            try
            {
                var apiUrl = "https://localhost:7207/odata/ProductField/get-by-id";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonSerializer.Deserialize<ProductFieldRequest>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var productfield = await _context.ProductFields.FindAsync(id);
            //if (productfield != null)
            //{
            //    ProductField = productfield;
            //    _context.ProductFields.Remove(ProductField);
            //    await _context.SaveChangesAsync();
            //}
            try
            {
                var api = "https://localhost:7207/odata/delete-by-id";
                var response = _httpClient.DeleteAsync(api);
                if (response.IsCompletedSuccessfully)
                {
                    await _hubContext.Clients.All.SendAsync("ProductFieldDeleted", api);
                    return RedirectToPage("/Index");
                }
                            
            ErrorMessage = $"Error deleting productField. Status code: {response.Status}";
            return Page();
        }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting task: {ex.Message}";
                return Page();
    }
}
    }
}


