using Microsoft.AspNetCore.Mvc;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;

namespace ReservasCanchas_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _service;
        public MetodosPagoController(IMetodoPagoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetodoPago>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MetodoPago>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<MetodoPago>> Create(MetodoPago model)
        {
            await _service.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.MetodoId }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, MetodoPago model)
        {
            if (id != model.MetodoId) return BadRequest();
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