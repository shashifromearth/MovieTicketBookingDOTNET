using MovieTicketBooking.Domain.Enums;

namespace MovieTicketBooking.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int ShowTimeId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public DateTime BookedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    public ShowTime ShowTime { get; set; } = null!;
    public ICollection<BookedSeat> Seats { get; set; } = [];
}
