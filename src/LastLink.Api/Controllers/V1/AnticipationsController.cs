using Asp.Versioning;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Enums;
using LastLink.Domain.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LastLink.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/antecipations")]
    public class AnticipationsController : BaseController
    {
        private readonly IAnticipationService _service;

        public AnticipationsController(IAnticipationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnticipationRequest dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.IsSuccess)
                return GetResultByErrorMessage(result.Errors);

            return Created(string.Empty, result.Value);
        }

        [HttpGet("by-creator/{creatorId}")]
        public async Task<IActionResult> ListByCreator(string creatorId)
        {
            var list = await _service.ListByCreatorAsync(creatorId);
            if (!list.IsSuccess)
                return GetResultByErrorMessage(list.Errors);

            return Created(string.Empty, list.Value);
        }

        [HttpGet("simulate")]
        public IActionResult Simulate([FromQuery] string creatorId, [FromQuery] decimal valorSolicitado)
        {
            var result = _service.Simulate(valorSolicitado, creatorId);
            if (!result.IsSuccess)
                return GetResultByErrorMessage(result.Errors);

            return Created(string.Empty, result.Value);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] AntecipationStatusEnum status)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(id, status);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    // small helper for CreatedAtAction
        //    var list = await _service.ListByCreatorAsync(""); // not ideal but used only for CreatedAtAction signature
        //    return NotFound();
        //}
    }
}
