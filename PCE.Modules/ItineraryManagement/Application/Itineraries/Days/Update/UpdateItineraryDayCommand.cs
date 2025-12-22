using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Update;

public record UpdateItineraryDayCommand(string UserSlug, string ItinerarySlug, Guid DayId, DateOnly Date) : IRequest<Result>;
