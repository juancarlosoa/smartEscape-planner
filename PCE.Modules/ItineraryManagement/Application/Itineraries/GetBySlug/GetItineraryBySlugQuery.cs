using MediatR;
using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.GetBySlug;

public record GetItineraryBySlugQuery(string UserSlug, string Slug) : IRequest<Result<ItineraryDto>>;
