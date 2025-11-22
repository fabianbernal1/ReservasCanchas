using Microsoft.AspNetCore.Mvc;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _service;
        public PagosController(IPagoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pago>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Pago>> Create(Pago model)
        {
            await _service.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.PagoId }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Pago model)
        {
            if (id != model.PagoId) return BadRequest();
            await _service.UpdateAsync(model);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}