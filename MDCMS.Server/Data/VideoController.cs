using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MDCMS.Server.Data;
using MDCMS.Server.Models;

namespace MDCMS.Server.Controllers
{
    [Route("api/v1/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _repo;

        public VideoController(IVideoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Video>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<List<Video>>> Search([FromQuery] string keyword) =>
            Ok(await _repo.SearchAsync(keyword));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Video>> GetById(string id)
        {
            var video = await _repo.GetByIdAsync(id);
            if (video == null) return NotFound();
            return Ok(video);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] Video video)
        {
            await _repo.CreateAsync(video);
            return CreatedAtAction(nameof(GetById), new { id = video.Id }, video);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(string id, [FromBody] Video video)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            video.Id = id;
            await _repo.UpdateAsync(video);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
