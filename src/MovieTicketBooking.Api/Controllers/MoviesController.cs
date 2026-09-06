using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Api.DTOs;
using MovieTicketBooking.Api.Mappings;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Interfaces;

namespace MovieTicketBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IRepository<Movie> movieRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetAll(CancellationToken cancellationToken)
    {
        var movies = await movieRepository.GetAllAsync(cancellationToken);
        return Ok(movies.Select(m => m.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(id, cancellationToken);
        return movie is null ? NotFound() : Ok(movie.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            Genre = request.Genre,
            Rating = request.Rating
        };

        await movieRepository.AddAsync(movie, cancellationToken);
        await movieRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MovieDto>> Update(int id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(id, cancellationToken);
        if (movie is null) return NotFound();

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.Genre = request.Genre;
        movie.Rating = request.Rating;

        movieRepository.Update(movie);
        await movieRepository.SaveChangesAsync(cancellationToken);

        return Ok(movie.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(id, cancellationToken);
        if (movie is null) return NotFound();

        movieRepository.Remove(movie);
        await movieRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
