using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntentoGoogleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using IntentoGoogleAPI.Models.DTO;

namespace IntentoGoogleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventariosController : ControllerBase
    {
        private readonly ContabilidadContext _context;

        public InventariosController(ContabilidadContext context)
        {
            _context = context;
        }

        // GET: api/Inventarios
        [Authorize(policy: "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<adminStockRes>>> GetInventariosAdmin()
        {
            if (_context.Inventarios == null)
            {
                return NotFound();
            }

            // esto antes devolvia inventarios completo

            var inventarios = await (from i in _context.Inventarios
                                     join p in _context.Productos on i.IdProducto equals p.IdProducto
                                     join t in _context.Tienda on i.IdTienda equals t.IntId
                                     select new adminStockRes
                                     {
                                         intId = i.IntId,
                                         CodigoProducto = i.IdProducto,
                                         Stock = i.Stock,
                                         stockMinimo = i.StockMinimo,
                                         nombreTienda = t.Localidad,
                                         nombreProducto = p.Nombre,
                                         idTienda = t.IntId
                                     }).ToListAsync();

            return inventarios;
        }


        // GET: api/Inventarios
        [HttpGet("tienda/{idTienda}"),Authorize(policy:"AdminOrPropietario")]
        public async Task<ActionResult<IEnumerable<adminStockRes>>> GetInventariosPropietario(int idTienda)
        {
            if (_context.Inventarios == null)
            {
                return NotFound();
            }
            var inventarios = await (from i in _context.Inventarios
                                     where i.IdTienda == idTienda
                                     join p in _context.Productos on i.IdProducto equals p.IdProducto
                                     join t in _context.Tienda on i.IdTienda equals t.IntId
                                     select new adminStockRes
                                     {
                                         intId = i.IntId,
                                         CodigoProducto = i.IdProducto,
                                         Stock = i.Stock,
                                         stockMinimo = i.StockMinimo,
                                         nombreTienda = t.Localidad,
                                         nombreProducto = p.Nombre,
                                         idTienda = t.IntId
                                     }).ToListAsync();

            return inventarios;
        }

        // GET: api/Inventarios/5
        [Authorize(policy: "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventario>> GetInventario(string id)
        {
            if (_context.Inventarios == null)
            {
                return NotFound();
            }
            var Inventario = await _context.Inventarios.FindAsync(id);

            if (Inventario == null)
            {
                return NotFound();
            }

            return Inventario;
        }

        // PUT: api/Inventarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"),Authorize(policy:"AdminOrPropietario")]
        public async Task<IActionResult> PutInventario(int id, Inventario inventario)
        {
            if (id != inventario.IntId)
            {
                return BadRequest();
            }

            _context.Entry(inventario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventarioExists(id))
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

        // POST: api/Inventarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost,Authorize(policy:"AdminOrPropietario")]
        public async Task<ActionResult<Inventario>> PostInventario(Inventario inventario)
        {
          if (_context.Inventarios == null)
          {
              return Problem("Entity set 'ContabilidadContext.Inventarios'  is null.");
          }
            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventario", new { id = inventario.IntId }, inventario);
        }

        // DELETE: api/Inventarios/5

        [HttpDelete("{id}"),Authorize(policy:"Admin")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            if (_context.Inventarios == null)
            {
                return NotFound();
            }
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }

            _context.Inventarios.Remove(inventario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventarioExists(int id)
        {
            return (_context.Inventarios?.Any(e => e.IntId == id)).GetValueOrDefault();
        }
    }
}
