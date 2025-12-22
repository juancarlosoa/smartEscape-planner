using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Add;

public record AddItineraryDayCommand(string UserSlug, string ItinerarySlug, DateOnly Date) : IRequest<Result<Guid>>;
