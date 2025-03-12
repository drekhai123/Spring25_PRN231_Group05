using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerFarmTaskManagementSystem.API.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        // GET: odata/Field/get-all-field
        [HttpGet("get-all-field")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<FieldDTO>>> GetAllFields()
        {
            var fields = await _fieldService.GetAllFieldsAsync();
            return Ok(fields);
        }

        // GET: odata/Field/by-id
        [HttpGet("by-id")]
        public async Task<ActionResult<FieldDTO>> GetFieldById(Guid id)
        {
            try
            {
                var field = await _fieldService.GetFieldByIdAsync(id);
                return Ok(field);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // POST: odata/Field/create-field
        [HttpPost("create-field")]
        public async Task<ActionResult<FieldDTO>> CreateField([FromBody] FieldCreateDTO fieldCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var field = await _fieldService.AddFieldAsync(fieldCreateDTO);
                return CreatedAtAction(nameof(GetFieldById), new { id = field.FieldId }, field);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: odata/Field/update-field
        [HttpPut("update-field")]
        public async Task<ActionResult<FieldDTO>> UpdateField(Guid id, [FromBody] FieldUpdateDTO fieldUpdateDTO)
        {
            if (id != fieldUpdateDTO.FieldId)
            {
                return BadRequest(new { Message = "Field ID mismatch." });
            }

            try
            {
                var updatedField = await _fieldService.UpdateFieldAsync(id, fieldUpdateDTO);
                return Ok(updatedField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: odata/Field/delete-field
        [HttpDelete("delete-field")]
        public async Task<ActionResult> DeleteField(Guid id)
        {
            try
            {
                await _fieldService.DeleteFieldAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
} 