using MediatR;
using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.GetAll;

public record GetAllItinerariesQuery(string UserSlug) : IRequest<Result<List<ItineraryListDto>>>;
