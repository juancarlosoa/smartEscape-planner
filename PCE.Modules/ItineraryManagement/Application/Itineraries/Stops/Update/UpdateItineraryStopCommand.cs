using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

public record UpdateItineraryStopCommand(string UserSlug, string ItinerarySlug, Guid StopId, string Notes, DateTime ScheduledTime) : IRequest<Result>;
