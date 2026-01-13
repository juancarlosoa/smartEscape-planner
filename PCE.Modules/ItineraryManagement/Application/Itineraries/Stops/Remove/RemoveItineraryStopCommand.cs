using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Remove;

public record RemoveItineraryStopCommand(string UserSlug, string ItinerarySlug, Guid StopId) : IRequest<Result>;
