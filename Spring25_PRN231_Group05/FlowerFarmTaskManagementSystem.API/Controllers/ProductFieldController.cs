using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class ProductFieldController : ControllerBase
    {
        private readonly IProductFieldService _productFieldService;

        public ProductFieldController(IProductFieldService productFieldService)
        {
            _productFieldService = productFieldService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductFieldDetailDTO>>> GetAllProductFields()
        {
            var productFields = await _productFieldService.GetAllProductFieldsAsync();
            return Ok(productFields);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductFieldDetailDTO>> GetProductFieldById(Guid id)
        {
            try
            {
                var productField = await _productFieldService.GetProductFieldByIdAsync(id);
                return Ok(productField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}