using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Delete;

public record DeleteItineraryCommand(string UserSlug, string Slug) : IRequest<Result>;
