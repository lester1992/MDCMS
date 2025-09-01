using MDCMS.Server.Data;
using MDCMS.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDCMS.Server.Controllers
{
    [ApiController]
    [Route("api/v1/kiosk")]
    public class KioskController : ControllerBase
    {
        private readonly IKioskRepository _kioskRepository;

        public KioskController(IKioskRepository kioskRepository)
        {
            _kioskRepository = kioskRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Kiosk>>> GetAll()
        {
            var kiosks = await _kioskRepository.GetAllAsync();
            return Ok(kiosks);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Kiosk>> GetById(string id)
        {
            var kiosk = await _kioskRepository.GetByIdAsync(id);
            if (kiosk == null)
                return NotFound();
            return Ok(kiosk);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(Kiosk kiosk)
        {
            await _kioskRepository.CreateAsync(kiosk);
            return CreatedAtAction(nameof(GetById), new { id = kiosk.Id }, kiosk);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(string id, Kiosk kiosk)
        {
            var existing = await _kioskRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            kiosk.Id = id;
            await _kioskRepository.UpdateAsync(kiosk);
            return Ok(new { message = "updated successfully", data = kiosk });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _kioskRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _kioskRepository.DeleteAsync(id);
            return Ok(new { message = "deleted successfully" });
        }
    }
}
