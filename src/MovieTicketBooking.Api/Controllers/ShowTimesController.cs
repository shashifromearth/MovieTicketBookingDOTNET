using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Api.DTOs;
using MovieTicketBooking.Api.Mappings;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Interfaces;

namespace MovieTicketBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowTimesController(
    IRepository<ShowTime> showTimeRepository,
    IShowTimeRepository showTimeRepo,
    IRepository<Movie> movieRepository,
    IRepository<Screen> screenRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowTimeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var showTimes = await showTimeRepo.GetAllWithDetailsAsync(cancellationToken);
        return Ok(showTimes.Select(st => st.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShowTimeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var showTime = await showTimeRepo.GetWithDetailsAsync(id, cancellationToken);
        return showTime is null ? NotFound() : Ok(showTime.ToDto());
    }

    [HttpGet("by-movie/{movieId:int}")]
    public async Task<ActionResult<IEnumerable<ShowTimeDto>>> GetByMovie(int movieId, CancellationToken cancellationToken)
    {
        var showTimes = await showTimeRepo.GetByMovieIdAsync(movieId, cancellationToken);
        return Ok(showTimes.Select(st => st.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<ShowTimeDto>> Create([FromBody] CreateShowTimeRequest request, CancellationToken cancellationToken)
    {
        if (await movieRepository.GetByIdAsync(request.MovieId, cancellationToken) is null)
            return BadRequest($"Movie {request.MovieId} not found.");

        if (await screenRepository.GetByIdAsync(request.ScreenId, cancellationToken) is null)
            return BadRequest($"Screen {request.ScreenId} not found.");

        var showTime = new ShowTime
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            BasePrice = request.BasePrice
        };

        await showTimeRepository.AddAsync(showTime, cancellationToken);
        await showTimeRepository.SaveChangesAsync(cancellationToken);

        var created = await showTimeRepo.GetWithDetailsAsync(showTime.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = showTime.Id }, created!.ToDto());
    }
}
