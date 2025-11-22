using Microsoft.AspNetCore.Mvc;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosReservaController : ControllerBase
    {
        private readonly IEstadoReservaService _service;
        public EstadosReservaController(IEstadoReservaService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadoReserva>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstadoReserva>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<EstadoReserva>> Create(EstadoReserva model)
        {
            await _service.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.EstadoId }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EstadoReserva model)
        {
            if (id != model.EstadoId) return BadRequest();
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