using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Api.DTOs;
using MovieTicketBooking.Api.Mappings;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Interfaces;

namespace MovieTicketBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatersController(
    IRepository<Theater> theaterRepository,
    IRepository<Screen> screenRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TheaterDto>>> GetAll(CancellationToken cancellationToken)
    {
        var theaters = await theaterRepository.GetAllAsync(cancellationToken);
        return Ok(theaters.Select(t => t.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TheaterDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var theater = await theaterRepository.GetByIdAsync(id, cancellationToken);
        return theater is null ? NotFound() : Ok(theater.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<TheaterDto>> Create([FromBody] CreateTheaterRequest request, CancellationToken cancellationToken)
    {
        var theater = new Theater
        {
            Name = request.Name,
            Location = request.Location
        };

        await theaterRepository.AddAsync(theater, cancellationToken);
        await theaterRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = theater.Id }, theater.ToDto());
    }

    [HttpGet("{theaterId:int}/screens")]
    public async Task<ActionResult<IEnumerable<ScreenDto>>> GetScreens(int theaterId, CancellationToken cancellationToken)
    {
        var screens = await screenRepository.GetAllAsync(cancellationToken);
        var theaterScreens = screens.Where(s => s.TheaterId == theaterId).Select(s => s.ToDto());
        return Ok(theaterScreens);
    }

    [HttpPost("screens")]
    public async Task<ActionResult<ScreenDto>> CreateScreen([FromBody] CreateScreenRequest request, CancellationToken cancellationToken)
    {
        var theater = await theaterRepository.GetByIdAsync(request.TheaterId, cancellationToken);
        if (theater is null) return BadRequest($"Theater {request.TheaterId} not found.");

        var screen = new Screen
        {
            TheaterId = request.TheaterId,
            Name = request.Name,
            TotalRows = request.TotalRows,
            SeatsPerRow = request.SeatsPerRow
        };

        await screenRepository.AddAsync(screen, cancellationToken);
        await screenRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetScreens), new { theaterId = screen.TheaterId }, screen.ToDto());
    }
}
