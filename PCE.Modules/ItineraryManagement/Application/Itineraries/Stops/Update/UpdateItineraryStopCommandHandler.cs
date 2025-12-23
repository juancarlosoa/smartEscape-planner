using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

public class UpdateItineraryStopCommandHandler : IRequestHandler<UpdateItineraryStopCommand, Result>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItineraryStopCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result.Failure("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found.");
        }

        var day = itinerary.ItineraryDays.FirstOrDefault(d => d.Id == request.DayId);
        if (day is null)
        {
            return Result.Failure("ItineraryDay.NotFound", $"ItineraryDay with id {request.DayId} was not found in itinerary {request.ItinerarySlug}.");
        }

        if (!day.ItineraryStops.Any(s => s.Id == request.StopId))
        {
            return Result.Failure("ItineraryStop.NotFound", $"ItineraryStop with id {request.StopId} was not found in day {request.DayId}.");
        }

        itinerary.UpdateStopInDay(request.DayId, request.StopId, request.Notes);

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
