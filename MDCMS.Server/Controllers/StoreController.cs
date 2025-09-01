using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MDCMS.Server.Data;
using MDCMS.Server.Models;

namespace MDCMS.Server.Controllers
{
    [Route("api/v1/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreRepository _repo;

        public StoreController(IStoreRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Store>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<List<Store>>> Search([FromQuery] string keyword) =>
            Ok(await _repo.SearchAsync(keyword));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Store>> GetById(string id)
        {
            var store = await _repo.GetByIdAsync(id);
            if (store == null) return NotFound();
            return Ok(store);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] Store store)
        {
            store.DateModified = DateTime.UtcNow;
            if (string.IsNullOrEmpty(store.StoreLogo))
                store.StoreLogo = "default-logo.png";

            await _repo.CreateAsync(store);
            return CreatedAtAction(nameof(GetById), new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(string id, [FromBody] Store store)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // preserve non-editable fields
            store.Id = id;
            store.StoreId = existing.StoreId;
            store.StoreLevel = existing.StoreLevel;
            store.DateModified = DateTime.UtcNow;

            await _repo.UpdateAsync(store);
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
