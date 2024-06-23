﻿using ApiMusic.Data;
using ApiMusic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private ApiDbContext _dbContext;
        public ArtistsController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST api/<SongsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Artist artist)
        {
            await _dbContext.Artists.AddAsync(artist);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        public async Task<IActionResult> GetArtists()
        {
            var artists = await (from artist in _dbContext.Artists
                          select new
                          {
                              Id = artist.Id,
                              Name = artist.Name
                          }).ToListAsync();
            return Ok(artists);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ArtistDetails(int artistId)
        {
            var artistDetails = await _dbContext.Artists.Where(a => a.Id == artistId).Include(a => a.Songs).ToListAsync();
            return Ok(artistDetails);
        }
    }
}
