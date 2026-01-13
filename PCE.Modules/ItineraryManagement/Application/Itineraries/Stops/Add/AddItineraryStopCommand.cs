using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Add;

public record AddItineraryStopCommand(
    string UserSlug,
    string ItinerarySlug,
    DateTime ScheduledTime,
    Guid EscapeRoomId,
    string Notes
) : IRequest<Result<Guid>>;
