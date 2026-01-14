using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCE.Shared.Extensions;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Create;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Remove;
using PCE.Modules.ItineraryManagement.Application.Itineraries.GetAll;
using PCE.Modules.ItineraryManagement.Application.Itineraries.GetBySlug;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Update;

namespace PCE.Modules.ItineraryManagement.Api.Itineraries;

[ApiController]
[Route("api/itineraries")]
public class ItineraryController : ControllerBase
{
    // DTO for binding body without UserSlug
    public record CreateItineraryRequest(
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate
    );

    private readonly IMediator _mediator;

    public ItineraryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new GetAllItinerariesQuery(userSlug), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new GetItineraryBySlugQuery(userSlug, slug), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItineraryRequest request, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();

        // Map DTO to Command, injecting the UserSlug
        var command = new CreateItineraryCommand(
            userSlug,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate
        );

        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string slug, [FromBody] UpdateItineraryCommand command, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();

        // Crear nuevo command con el userSlug autenticado
        var secureCommand = command with { UserSlug = userSlug, Slug = slug };
        var result = await _mediator.Send(secureCommand, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Remove(string slug, CancellationToken ct)
    {
        var userSlug = this.GetUserSlug();
        var result = await _mediator.Send(new RemoveItineraryCommand(userSlug, slug), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
