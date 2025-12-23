using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCE.Shared.Extensions;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Add;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Remove;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

namespace PCE.Modules.ItineraryManagement.Api.Itineraries;

[ApiController]
[Route("api/days/{dayId:guid}/stops")]
public class ItineraryStopController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItineraryStopController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddStop(Guid dayId, [FromBody] AddItineraryStopCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, DayId = dayId };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{stopId:guid}")]
    public async Task<IActionResult> UpdateStop(Guid dayId, Guid stopId, [FromBody] UpdateItineraryStopCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, DayId = dayId, StopId = stopId };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{stopId:guid}")]
    public async Task<IActionResult> RemoveStop(Guid dayId, Guid stopId, [FromQuery] string itinerarySlug, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new RemoveItineraryStopCommand(userSlug, itinerarySlug, dayId, stopId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
