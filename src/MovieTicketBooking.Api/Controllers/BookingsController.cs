using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Api.DTOs;
using MovieTicketBooking.Api.Mappings;
using MovieTicketBooking.Domain.Interfaces;

namespace MovieTicketBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(
    IBookingService bookingService,
    IBookingRepository bookingRepository) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetWithDetailsAsync(id, cancellationToken);
        return booking is null ? NotFound() : Ok(booking.ToDto());
    }

    [HttpGet("showtime/{showTimeId:int}/seats")]
    public async Task<ActionResult<IEnumerable<SeatAvailabilityDto>>> GetSeatAvailability(
        int showTimeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var availability = await bookingService.GetSeatAvailabilityAsync(showTimeId, cancellationToken);
            return Ok(availability.Select(s => new SeatAvailabilityDto(s.Row, s.Number, s.IsAvailable)));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var seats = request.Seats.Select(s => (s.Row, s.Number)).ToList();
            var booking = await bookingService.CreateBookingAsync(
                request.ShowTimeId,
                request.CustomerName,
                request.CustomerEmail,
                seats,
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bookingService.CancelBookingAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
