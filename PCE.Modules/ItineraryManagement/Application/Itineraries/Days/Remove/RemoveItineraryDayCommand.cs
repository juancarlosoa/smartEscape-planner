using MediatR;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Remove;

public record RemoveItineraryDayCommand(string UserSlug, string ItinerarySlug, Guid DayId) : IRequest<Result>;
