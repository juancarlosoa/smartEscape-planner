using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Remove;

public class RemoveItineraryDayCommandHandler : IRequestHandler<RemoveItineraryDayCommand, Result>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItineraryDayCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveItineraryDayCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result.Failure(Error.NotFound("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found."));
        }

        if (!itinerary.ItineraryDays.Any(d => d.Id == request.DayId))
        {
            return Result.Failure(Error.NotFound("ItineraryDay.NotFound", $"ItineraryDay with id {request.DayId} was not found in itinerary {request.ItinerarySlug}."));
        }

        itinerary.RemoveDay(request.DayId);

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
