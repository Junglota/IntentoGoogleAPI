using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntentoGoogleAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace IntentoGoogleAPI.Controllers
{
    [Authorize(policy:"Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TiendumsController : ControllerBase
    {
        private readonly ContabilidadContext _context;

        public TiendumsController(ContabilidadContext context)
        {
            _context = context;
        }

        // GET: api/Tiendums
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tiendum>>> GetTienda()
        {
          if (_context.Tienda == null)
          {
              return NotFound();
          }
            return await _context.Tienda.ToListAsync();
        }

        // GET: api/Tiendums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tiendum>> GetTiendum(int id)
        {
          if (_context.Tienda == null)
          {
              return NotFound();
          }
            var tiendum = await _context.Tienda.FindAsync(id);

            if (tiendum == null)
            {
                return NotFound();
            }

            return tiendum;
        }

        // PUT: api/Tiendums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTiendum(int id, Tiendum tiendum)
        {
            if (id != tiendum.IntId)
            {
                return BadRequest();
            }

            _context.Entry(tiendum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TiendumExists(id))
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

        // POST: api/Tiendums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tiendum>> PostTiendum(Tiendum tiendum)
        {
          if (_context.Tienda == null)
          {
              return Problem("Entity set 'ContabilidadContext.Tienda'  is null.");
          }
            _context.Tienda.Add(tiendum);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTiendum", new { id = tiendum.IntId }, tiendum);
        }

        // DELETE: api/Tiendums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTiendum(int id)
        {
            if (_context.Tienda == null)
            {
                return NotFound();
            }
            var tiendum = await _context.Tienda.FindAsync(id);
            if (tiendum == null)
            {
                return NotFound();
            }

            _context.Tienda.Remove(tiendum);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {

            }
            

            return NoContent();
        }

        private bool TiendumExists(int id)
        {
            return (_context.Tienda?.Any(e => e.IntId == id)).GetValueOrDefault();
        }
    }
}
