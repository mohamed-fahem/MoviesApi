using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper  _mapper;
        private readonly IMoviesServices _moviesServices;
        private readonly IGenresServices _genresServices;



        public MoviesController(IMoviesServices moviesServices, IGenresServices genresServices, IMapper mapper)
        {
            _moviesServices = moviesServices;
            _genresServices = genresServices;
            _mapper = mapper;
        }

        private new List<string> _allowedExtenstions = new List<string>{".jpg",".png" };
        private int _maxAllowedPosterSize = 1048576;
        

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesServices.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _moviesServices.GetById(id);

            if (movie == null)
                return NotFound();
            var data = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(data);

        }

        [HttpGet("GetByGenreId")]

        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesServices.GetAll(genreId);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {

            if (dto.Poster == null) return BadRequest("The poster is required !");

            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("only .jpg and .png are allowed! ");

            if(dto.Poster.Length >_maxAllowedPosterSize)
                return BadRequest("Max size allowed is 1MB!");

            var isValidGenre = await _genresServices.IsvalidGenra(dto.GenreId);

            if(!isValidGenre)
                return BadRequest("Invalid Genre ID !");

            using var datastream = new MemoryStream();
            await dto.Poster.CopyToAsync(datastream);

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster= datastream.ToArray();

            await _moviesServices.Add(movie);
           
            return Ok(movie);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdataAsync(int id,[FromForm]MovieDto dto)
        {
            var movie = await _moviesServices.GetById(id);
            
            if (movie == null) return BadRequest();
            
            
            var isValidGenre = await _genresServices.IsvalidGenra(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID !");

            if(dto.Poster != null)
            {
                if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("only .jpg and .png are allowed! ");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max size allowed is 1MB!");
                
                using var datastream = new MemoryStream();
                await dto.Poster.CopyToAsync(datastream);

                movie.Poster = datastream.ToArray();
            }

           

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Storeline = dto.Storeline;
            movie.Rate = dto.Rate;
            movie.GenreId = dto.GenreId;

            _moviesServices.Update(movie);
            return Ok(movie);



        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie == null) return BadRequest($"No movie was found with ID :{id}");
            _moviesServices.Delete(movie);

            return Ok(movie);
        }

    }
}
