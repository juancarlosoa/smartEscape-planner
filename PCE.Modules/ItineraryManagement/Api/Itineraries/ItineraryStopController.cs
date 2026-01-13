using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCE.Shared.Extensions;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Add;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Remove;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

namespace PCE.Modules.ItineraryManagement.Api.Itineraries;

[ApiController]
[Route("api/itineraries/{itinerarySlug}/stops")]
public class ItineraryStopController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItineraryStopController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddStop(string itinerarySlug, [FromBody] AddItineraryStopCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, ItinerarySlug = itinerarySlug };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{stopId:guid}")]
    public async Task<IActionResult> UpdateStop(string itinerarySlug, Guid stopId, [FromBody] UpdateItineraryStopCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, ItinerarySlug = itinerarySlug, StopId = stopId };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{stopId:guid}")]
    public async Task<IActionResult> RemoveStop(string itinerarySlug, Guid stopId, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new RemoveItineraryStopCommand(userSlug, itinerarySlug, stopId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
