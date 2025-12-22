using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Create;

public record CreateItineraryCommand(
    string UserSlug,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<string>>;
