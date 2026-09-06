namespace MovieTicketBooking.Domain.Entities;

public class BookedSeat
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public required string Row { get; set; }
    public int Number { get; set; }
    public decimal Price { get; set; }

    public Booking Booking { get; set; } = null!;
}
