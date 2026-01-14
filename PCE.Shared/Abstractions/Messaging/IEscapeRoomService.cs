namespace PCE.Shared.Abstractions.Messaging;

public interface IEscapeRoomService
{
    Task<Guid?> GetEscapeRoomIdBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<string?> GetEscapeRoomSlugByIdAsync(Guid id, CancellationToken cancellationToken = default);
}