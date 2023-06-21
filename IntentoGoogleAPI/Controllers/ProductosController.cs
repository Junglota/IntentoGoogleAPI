using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntentoGoogleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;

namespace IntentoGoogleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ContabilidadContext _context;

        public ProductosController(ContabilidadContext context)
        {
            _context = context;
        }


        // GET: api/Productos
        [Authorize(policy: "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosAdmin()
        {
          if (_context.Productos == null)
          {
              return NotFound();
          }
            return await _context.Productos.ToListAsync();
        }


        // GET: api/Productos
        [Route("/tienda/{idTienda}")]
        [Authorize(policy: "Admin,Propietario,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPropietario(int idTienda)
        {
          if (_context.Productos == null)
          {
              return NotFound();
          }
            return await _context.Productos.Where(p => p.IdTienda == idTienda).ToListAsync();
        }
        
        // GET: api/Productos/5
        [Authorize(policy: "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(string id)
        {
          if (_context.Productos == null)
          {
              return NotFound();
          }
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(string id, Producto producto)
        {
            if (id != producto.Cod)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(policy: "Admin,Propietario")]
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
          if (_context.Productos == null)
          {
              return Problem("Entity set 'ContabilidadContext.Productos'  is null.");
          }
            _context.Productos.Add(producto);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductoExists(producto.Cod))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProducto", new { id = producto.Cod }, producto);
        }

        // DELETE: api/Productos/5
        [Authorize(policy: "Admin,Propietario")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(string id)
        {
            if (_context.Productos == null)
            {
                return NotFound();
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(policy: "Admin,Propietario")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] JsonPatchDocument<Producto> personPatch) // Ejemplo de body: [{"op" : "replace", "path" : "/Cantidad", "value" : "10"}]
        {
            var result = _context.Productos.FirstOrDefault(n => n.Cod == id);
            if (result == null)
            {
                return BadRequest();

            }
            personPatch.ApplyTo(result);
            _context.Entry(result).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        private bool ProductoExists(string id)
        {
            return (_context.Productos?.Any(e => e.Cod == id)).GetValueOrDefault();
        }
    }
}
