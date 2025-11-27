using FluentResults;
using LastLink.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace LastLink.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly IEnumerable<string> NotFoundErrors = new[]
        {
            ErrorMessages.CREATOR_SEM_SOLICITACOES.Message,
            ErrorMessages.SOLICITACAO_NAO_ENCONTRADA.Message
        };

        protected IActionResult GetResultByErrorMessage(IReadOnlyList<IError> error)
        {
            if (error.Any(e => NotFoundErrors.Contains(e.Message)))
                return NotFound(error);

            return BadRequest(error);
        }
    }
}
