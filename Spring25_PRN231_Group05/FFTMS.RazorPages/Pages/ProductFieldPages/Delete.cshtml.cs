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
            
            if (id == null)
            {
                return NotFound();
            }
            var apiUrl = $"http://localhost:5281/odata/ProductFields/get-by-id?id={id}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();


            ProductField parsedResponse;

            parsedResponse = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });



            if (parsedResponse == null)
            {
                return NotFound();
            }

            ProductField = parsedResponse;
            return Page();
        
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            
            try
            {
                var api = $"https://localhost:5281/odata/ProductFields/delete-by-id?id={id}";
                var response = await _httpClient.DeleteAsync(api);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    await _hubContext.Clients.All.SendAsync("ProductFieldDeleted", api);
                    ProductField parsedResponse;

                    parsedResponse = JsonSerializer.Deserialize<ProductField>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return RedirectToPage("/Index");
                }
                            
            ErrorMessage = $"Error deleting productField. Status code: {response.StatusCode}";
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


