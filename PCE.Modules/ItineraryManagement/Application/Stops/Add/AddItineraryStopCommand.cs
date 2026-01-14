using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Stops.Add;

public record AddItineraryStopCommand(
    string UserSlug,
    string ItinerarySlug,
    DateTime ScheduledTime,
    string EscapeRoomSlug,
    string Notes
) : IRequest<Result<string>>;
