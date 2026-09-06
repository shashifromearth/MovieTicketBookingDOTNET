using MovieTicketBooking.Domain.Entities;

namespace MovieTicketBooking.Domain.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
