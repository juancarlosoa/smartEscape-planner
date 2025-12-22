using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Days.Add;

public class AddItineraryDayCommandHandler : IRequestHandler<AddItineraryDayCommand, Result<Guid>>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddItineraryDayCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddItineraryDayCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result<Guid>.Failure(Error.NotFound("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found."));
        }

        var day = itinerary.AddDay(request.Date);

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(day.Id);
    }
}
