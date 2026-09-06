using MovieTicketBooking.Domain.Entities;

namespace MovieTicketBooking.Domain.Interfaces;

public interface IShowTimeRepository : IRepository<ShowTime>
{
    Task<IReadOnlyList<ShowTime>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShowTime>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
    Task<ShowTime?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(string Row, int Number)>> GetBookedSeatsAsync(int showTimeId, CancellationToken cancellationToken = default);
}
