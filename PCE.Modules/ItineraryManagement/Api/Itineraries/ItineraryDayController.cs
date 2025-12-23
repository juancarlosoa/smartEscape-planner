using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCE.Shared.Extensions;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Add;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Remove;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Update;

namespace PCE.Modules.ItineraryManagement.Api.Itineraries;

[ApiController]
[Route("api/itineraries/{itinerarySlug}/days")]
public class ItineraryDayController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItineraryDayController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddDay(string itinerarySlug, [FromBody] AddItineraryDayCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, ItinerarySlug = itinerarySlug };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{dayId:guid}")]
    public async Task<IActionResult> UpdateDay(string itinerarySlug, Guid dayId, [FromBody] UpdateItineraryDayCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var secureCommand = command with { UserSlug = userSlug, ItinerarySlug = itinerarySlug, DayId = dayId };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{dayId:guid}")]
    public async Task<IActionResult> RemoveDay(string itinerarySlug, Guid dayId, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new RemoveItineraryDayCommand(userSlug, itinerarySlug, dayId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
