using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Interfaces;
using MovieTicketBooking.Infrastructure.Data;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class BookingRepository(AppDbContext context) : Repository<Booking>(context), IBookingRepository
{
    public async Task<Booking?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(b => b.Seats)
            .Include(b => b.ShowTime)
            .ThenInclude(st => st.Movie)
            .Include(b => b.ShowTime)
            .ThenInclude(st => st.Screen)
            .ThenInclude(s => s.Theater)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}
