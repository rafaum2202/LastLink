using Asp.Versioning;
using LastLink.Domain.Contracts.Services;
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
        public async Task<IActionResult> Create([FromBody] CreateAnticipationRequest request)
        {
            var result = await _service.CreateAsync(request);
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

            return Ok(list.Value);
        }

        [HttpGet("simulate")]
        public IActionResult Simulate([FromQuery] SimulateRequest request)
        {
            var result = _service.Simulate(request);
            if (!result.IsSuccess)
                return GetResultByErrorMessage(result.Errors);

            return Ok(result.Value);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromQuery] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateStatusAsync(id, request);
            if (!result.IsSuccess)
                return GetResultByErrorMessage(result.Errors);

            return Ok(result.Value);
        }
    }
}