using MediatR;
using PCE.Modules.EscapeManagement.Domain.EscapeRooms.Repositories;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Stops.Add;

public class AddItineraryStopCommandHandler : IRequestHandler<AddItineraryStopCommand, Result<string>>
{
    private readonly IItineraryRepository _itineraryRepository;
    private readonly IItineraryStopRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEscapeRoomRepository _escapeRoomRepository;

    public AddItineraryStopCommandHandler(
        IItineraryStopRepository repository,
        IItineraryRepository itineraryRepository,
        IEscapeRoomRepository escapeRoomRepository,
         IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _itineraryRepository = itineraryRepository;
        _unitOfWork = unitOfWork;
        _escapeRoomRepository = escapeRoomRepository;
    }

    public async Task<Result<string>> Handle(AddItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _itineraryRepository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result<string>.Failure("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found.");
        }

        var escapeRoom = await _escapeRoomRepository.GetBySlugAsync(request.EscapeRoomSlug, cancellationToken);
        if (escapeRoom is null)
        {
            return Result<string>.Failure("EscapeRoom.NotFound", $"Escape room with slug {request.EscapeRoomSlug} was not found.");
        }

        var stop = ItineraryStop.Create(
            itinerary.Id,
            escapeRoom.Id,
            request.Notes,
            request.ScheduledTime
        );

        await _repository.AddAsync(stop, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(stop.Id.ToString());
    }
}
