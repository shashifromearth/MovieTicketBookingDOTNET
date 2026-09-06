using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Enums;
using MovieTicketBooking.Domain.Interfaces;
using MovieTicketBooking.Infrastructure.Data;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class ShowTimeRepository(AppDbContext context) : Repository<ShowTime>(context), IShowTimeRepository
{
    public async Task<IReadOnlyList<ShowTime>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(st => st.Movie)
            .Include(st => st.Screen)
            .ThenInclude(s => s.Theater)
            .OrderBy(st => st.StartTime)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ShowTime>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(st => st.Movie)
            .Include(st => st.Screen)
            .ThenInclude(s => s.Theater)
            .Where(st => st.MovieId == movieId)
            .OrderBy(st => st.StartTime)
            .ToListAsync(cancellationToken);

    public async Task<ShowTime?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(st => st.Movie)
            .Include(st => st.Screen)
            .ThenInclude(s => s.Theater)
            .FirstOrDefaultAsync(st => st.Id == id, cancellationToken);

    public async Task<IReadOnlyList<(string Row, int Number)>> GetBookedSeatsAsync(
        int showTimeId,
        CancellationToken cancellationToken = default)
    {
        var seats = await Context.BookedSeats
            .Where(bs => bs.Booking.ShowTimeId == showTimeId && bs.Booking.Status == BookingStatus.Confirmed)
            .Select(bs => new { bs.Row, bs.Number })
            .ToListAsync(cancellationToken);

        return seats.Select(s => (s.Row, s.Number)).ToList();
    }
}
