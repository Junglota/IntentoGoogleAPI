using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntentoGoogleAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace IntentoGoogleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly ContabilidadContext _context;

        public MovimientosController(ContabilidadContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet,Authorize(policy: "Admin")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
        {
            if (_context.Movimientos == null)
            {
                return NotFound();
            }
            return await _context.Movimientos.ToListAsync();
        }
        [HttpGet("tienda/{idtienda}")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos(int idtienda)
        {
            var movimientos = await _context.Movimientos.Where(m => m.IdTienda == idtienda).ToListAsync();
            if (movimientos == null)
            {
                return NotFound();
            }
            return movimientos;
        }

        // GET: api/Movimientos/5

        [HttpGet("{id}"),Authorize(policy:"Admin")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
        {
            if (_context.Movimientos == null)
            {
                return NotFound();
            }
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return movimiento;
        }

        // PUT: api/Movimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"),Authorize(policy: "AdminOrPropietario")]
        public async Task<IActionResult> PutMovimiento(int id, Movimiento movimiento)
        {
            if (id != movimiento.IntId)
            {
                return BadRequest();
            }

            _context.Entry(movimiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("movimientoentrada")]
        public async Task<ActionResult<Movimiento>> MovimientoEntrada(Movimiento movimiento)
        {
            if (_context.Movimientos == null)
            {
                return Problem("Entity set 'ContabilidadContext.Movimientos'  is null.");
            }
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == movimiento.IdTienda && p.IdProducto == movimiento.IdProducto);
            if(inventario == null)
            {
                return Problem("El producto no existe");
            }
            _context.Movimientos.Add(movimiento);
            inventario.Stock += movimiento.Cantidad;
            _context.Entry(inventario).State = EntityState.Modified;
            try
            {

                await _context.SaveChangesAsync();
            }
            catch
            {
                return Problem("Hubo un problema agregando el registro");
            }
            

            return CreatedAtAction("GetMovimiento", new { id = movimiento.IntId }, movimiento);
        }
        [HttpPost("movimientosalida")]
        public async Task<ActionResult<Movimiento>> MovimientoSalida(Movimiento movimiento)
        {
            if (_context.Movimientos == null)
            {
                return Problem("Entity set 'ContabilidadContext.Movimientos'  is null.");
            }
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == movimiento.IdTienda && p.IdProducto == movimiento.IdProducto);
            if (inventario == null)
            {
                return Problem("El producto no existe");
            }
            _context.Movimientos.Add(movimiento);
            inventario.Stock -= movimiento.Cantidad;
            _context.Entry(inventario).State = EntityState.Modified;
            try
            {

                await _context.SaveChangesAsync();
            }
            catch
            {
                return Problem("Hubo un problema agregando el registro");
            }


            return CreatedAtAction("GetMovimiento", new { id = movimiento.IntId }, movimiento);
        }
        // DELETE: api/Movimientos/5
        [HttpDelete("{id}"),Authorize(policy:"AdminOrPropietario")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            if (_context.Movimientos == null)
            {
                return NotFound();
            }
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}"), Authorize(policy: "AdminOrPropietario")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Movimiento> personPatch) // Ejemplo de body: [{"op" : "replace", "path" : "/Cantidad", "value" : "10"}]
        {
            var result = _context.Movimientos.FirstOrDefault(n => n.IntId == id);
            if (result == null)
            {
                return BadRequest();

            }
            personPatch.ApplyTo(result);
            _context.Entry(result).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(result);
        }
        private bool MovimientoExists(int id)
        {
            return (_context.Movimientos?.Any(e => e.IntId == id)).GetValueOrDefault();
        }
    }
}
