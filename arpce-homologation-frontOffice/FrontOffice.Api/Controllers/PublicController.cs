using FrontOffice.Application.Features.Public.Commands.SendContactMail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly ISender _mediator;
    public PublicController(ISender sender)
    {
        sender = _mediator;
    }

    [HttpPost("contact")]
    public async Task<IActionResult> SendContactMail([FromBody] SendContactMailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }
}