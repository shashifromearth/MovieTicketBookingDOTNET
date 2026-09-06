using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Enums;
using MovieTicketBooking.Domain.Interfaces;
using MovieTicketBooking.Infrastructure.Data;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBooking.Infrastructure.Services;

namespace MovieTicketBooking.Tests;

public class BookingServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var showTimeRepo = new ShowTimeRepository(_context);
        var bookingRepo = new BookingRepository(_context);
        _sut = new BookingService(showTimeRepo, bookingRepo);
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidSeats_CreatesBooking()
    {
        var booking = await _sut.CreateBookingAsync(
            1, "John Doe", "john@example.com",
            [("A", 1), ("A", 2)]);

        Assert.NotNull(booking);
        Assert.Equal(2, booking.Seats.Count);
        Assert.Equal(25.00m, booking.TotalAmount);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public async Task CreateBookingAsync_WithBookedSeat_Throws()
    {
        await _sut.CreateBookingAsync(1, "Jane", "jane@example.com", [("B", 1)]);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateBookingAsync(1, "Bob", "bob@example.com", [("B", 1)]));

        Assert.Contains("already booked", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidSeat_Throws()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateBookingAsync(1, "Jane", "jane@example.com", [("Z", 1)]));

        Assert.Contains("out of range", ex.Message);
    }

    [Fact]
    public async Task CancelBookingAsync_CancelsConfirmedBooking()
    {
        var booking = await _sut.CreateBookingAsync(1, "Jane", "jane@example.com", [("C", 1)]);

        await _sut.CancelBookingAsync(booking.Id);

        var cancelled = await _context.Bookings.FindAsync(booking.Id);
        Assert.Equal(BookingStatus.Cancelled, cancelled!.Status);
    }

    [Fact]
    public async Task GetSeatAvailabilityAsync_ReturnsCorrectAvailability()
    {
        await _sut.CreateBookingAsync(1, "Jane", "jane@example.com", [("A", 1)]);

        var availability = await _sut.GetSeatAvailabilityAsync(1);

        Assert.Equal(40, availability.Count);
        Assert.Contains(availability, s => s.Row == "A" && s.Number == 1 && !s.IsAvailable);
        Assert.Contains(availability, s => s.Row == "A" && s.Number == 2 && s.IsAvailable);
    }

    public void Dispose() => _context.Dispose();
}
