using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

public record UpdateItineraryStopCommand(string UserSlug, string ItinerarySlug, Guid DayId, Guid StopId, string Notes) : IRequest<Result>;
