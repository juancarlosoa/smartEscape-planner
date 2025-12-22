using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Update;

public record UpdateItineraryCommand(
    string UserSlug,
    string Slug,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<string>>;
