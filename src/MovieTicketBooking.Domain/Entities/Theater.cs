namespace MovieTicketBooking.Domain.Entities;

public class Theater
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }

    public ICollection<Screen> Screens { get; set; } = [];
}
