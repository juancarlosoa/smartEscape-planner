using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Remove;

public record RemoveItineraryCommand(string UserSlug, string Slug) : IRequest<Result>;
