using Microsoft.AspNetCore.Mvc;
using ReservasCanchas_Web.Models;
using ReservasCanchas_Web.Servicios.Interfaces;
using ReservasCanchas_Web.Servicios.Exceptions; // <-- para ReservaSolapadaException
using System;

namespace ReservasCanchas_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _service;
        public ReservasController(IReservaService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Reserva>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reserva model)
        {
            try
            {
                await _service.AddAsync(model);
                return CreatedAtAction(nameof(Get), new { id = model.ReservaId }, model);
            }
            catch (ReservaSolapadaException ex)
            {
                // Retornar 400 con mensaje claro para que el front lo muestre
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                // Opcional: loggear antes de retornar 500
                return StatusCode(500, new { error = "Error al crear la reserva." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Reserva model)
        {
            if (id != model.ReservaId) return BadRequest();

            try
            {
                await _service.UpdateAsync(model);
                return NoContent();
            }
            catch (ReservaSolapadaException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error al actualizar la reserva." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}