using MovieTicketBooking.Domain.Entities;

namespace MovieTicketBooking.Domain.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(
        int showTimeId,
        string customerName,
        string customerEmail,
        IReadOnlyList<(string Row, int Number)> seats,
        CancellationToken cancellationToken = default);

    Task CancelBookingAsync(int bookingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string Row, int Number, bool IsAvailable)>> GetSeatAvailabilityAsync(
        int showTimeId,
        CancellationToken cancellationToken = default);
}
