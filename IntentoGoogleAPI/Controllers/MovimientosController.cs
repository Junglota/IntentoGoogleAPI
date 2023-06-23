using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntentoGoogleAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace IntentoGoogleAPI.Controllers
{
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
        {
          if (_context.Movimientos == null)
          {
              return NotFound();
          }
            return await _context.Movimientos.ToListAsync();
        }

        // GET: api/Movimientos/5
        [HttpGet("{id}")]
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
        [HttpPut("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        {
          if (_context.Movimientos == null)
          {
              return Problem("Entity set 'ContabilidadContext.Movimientos'  is null.");
          }
            Movimiento body = new Movimiento()
            {
                Fecha = movimiento.Fecha ?? DateTime.Now,
                IdProducto = movimiento.IdProducto,
                Cantidad = _context.Inventarios.Where(i => i.IdProducto == movimiento.IdProducto && i.IdTienda == movimiento.IdTienda).ToList().FirstOrDefault().Stock += movimiento.Cantidad,
                TipoMovimiento = movimiento.TipoMovimiento,
                Descripcion = movimiento.Descripcion,
                Usuario = movimiento.Usuario,
                IdTienda = movimiento.IdTienda
            };
            _context.Movimientos.Add(body);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovimiento", new { id = movimiento.IntId }, movimiento);
        }

        // DELETE: api/Movimientos/5
        [HttpDelete("{id}")]
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

        [HttpPatch("{id}")]
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
