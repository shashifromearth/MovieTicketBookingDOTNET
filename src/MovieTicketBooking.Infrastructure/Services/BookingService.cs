using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Enums;
using MovieTicketBooking.Domain.Interfaces;

namespace MovieTicketBooking.Infrastructure.Services;

public class BookingService(
    IShowTimeRepository showTimeRepository,
    IBookingRepository bookingRepository) : IBookingService
{
    public async Task<Booking> CreateBookingAsync(
        int showTimeId,
        string customerName,
        string customerEmail,
        IReadOnlyList<(string Row, int Number)> seats,
        CancellationToken cancellationToken = default)
    {
        if (seats.Count == 0)
            throw new InvalidOperationException("At least one seat must be selected.");

        var showTime = await showTimeRepository.GetWithDetailsAsync(showTimeId, cancellationToken)
            ?? throw new InvalidOperationException($"ShowTime {showTimeId} not found.");

        ValidateSeats(showTime.Screen, seats);

        var bookedSeats = await showTimeRepository.GetBookedSeatsAsync(showTimeId, cancellationToken);
        var bookedSet = bookedSeats.ToHashSet();

        var duplicateInRequest = seats.GroupBy(s => s).FirstOrDefault(g => g.Count() > 1);
        if (duplicateInRequest is not null)
            throw new InvalidOperationException($"Duplicate seat in request: {duplicateInRequest.Key.Row}{duplicateInRequest.Key.Number}.");

        foreach (var seat in seats)
        {
            if (bookedSet.Contains(seat))
                throw new InvalidOperationException($"Seat {seat.Row}{seat.Number} is already booked.");
        }

        var booking = new Booking
        {
            ShowTimeId = showTimeId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed,
            Seats = seats.Select(s => new BookedSeat
            {
                Row = s.Row.ToUpperInvariant(),
                Number = s.Number,
                Price = showTime.BasePrice
            }).ToList()
        };

        booking.TotalAmount = booking.Seats.Sum(s => s.Price);

        await bookingRepository.AddAsync(booking, cancellationToken);
        await bookingRepository.SaveChangesAsync(cancellationToken);

        return (await bookingRepository.GetWithDetailsAsync(booking.Id, cancellationToken))!;
    }

    public async Task CancelBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new InvalidOperationException($"Booking {bookingId} not found.");

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        booking.Status = BookingStatus.Cancelled;
        bookingRepository.Update(booking);
        await bookingRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(string Row, int Number, bool IsAvailable)>> GetSeatAvailabilityAsync(
        int showTimeId,
        CancellationToken cancellationToken = default)
    {
        var showTime = await showTimeRepository.GetWithDetailsAsync(showTimeId, cancellationToken)
            ?? throw new InvalidOperationException($"ShowTime {showTimeId} not found.");

        var bookedSeats = await showTimeRepository.GetBookedSeatsAsync(showTimeId, cancellationToken);
        var bookedSet = bookedSeats.ToHashSet();

        var availability = new List<(string Row, int Number, bool IsAvailable)>();
        for (var rowIndex = 0; rowIndex < showTime.Screen.TotalRows; rowIndex++)
        {
            var row = ((char)('A' + rowIndex)).ToString();
            for (var seatNum = 1; seatNum <= showTime.Screen.SeatsPerRow; seatNum++)
            {
                var isAvailable = !bookedSet.Contains((row, seatNum));
                availability.Add((row, seatNum, isAvailable));
            }
        }

        return availability;
    }

    private static void ValidateSeats(Screen screen, IReadOnlyList<(string Row, int Number)> seats)
    {
        foreach (var (row, number) in seats)
        {
            if (string.IsNullOrWhiteSpace(row) || row.Length != 1 || !char.IsLetter(row[0]))
                throw new InvalidOperationException($"Invalid row '{row}'. Use a single letter (A-{GetMaxRow(screen.TotalRows)}).");

            var rowIndex = char.ToUpperInvariant(row[0]) - 'A';
            if (rowIndex < 0 || rowIndex >= screen.TotalRows)
                throw new InvalidOperationException($"Row '{row}' is out of range for this screen.");

            if (number < 1 || number > screen.SeatsPerRow)
                throw new InvalidOperationException($"Seat number {number} is out of range for row {row}.");
        }
    }

    private static char GetMaxRow(int totalRows) => (char)('A' + totalRows - 1);
}
