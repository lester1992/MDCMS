using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MDCMS.Server.Data;
using MDCMS.Server.Models;

namespace MDCMS.Server.Controllers
{
    [Route("api/v1/image-ads")]
    [ApiController]
    public class ImageAdsController : ControllerBase
    {
        private readonly IImageAdsRepository _repo;

        public ImageAdsController(IImageAdsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ImageAds>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<List<ImageAds>>> Search([FromQuery] string keyword) =>
            Ok(await _repo.SearchAsync(keyword));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ImageAds>> GetById(string id)
        {
            var ad = await _repo.GetByIdAsync(id);
            if (ad == null) return NotFound();
            return Ok(ad);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] ImageAds ad)
        {
            await _repo.CreateAsync(ad);
            return CreatedAtAction(nameof(GetById), new { id = ad.Id }, ad);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(string id, [FromBody] ImageAds ad)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            ad.Id = id;
            await _repo.UpdateAsync(ad);
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
