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
using Microsoft.AspNetCore.JsonPatch.Operations;
using IntentoGoogleAPI.Services;

namespace IntentoGoogleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly ContabilidadContext _context;
        private readonly LoginService _loginService;

        public MovimientosController(ContabilidadContext context, LoginService loginService)
        {
            _context = context;
            _loginService = loginService;
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
        [HttpGet("tienda/{idtienda}"),Authorize(policy:"AdminOrPropietario")]
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
            var result = _context.Movimientos.AsNoTracking().FirstOrDefault(n => n.IntId == movimiento.IntId);
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == movimiento.IdTienda && p.IdProducto == movimiento.IdProducto);
            if (result is null || inventario is null)
            {
                return BadRequest();
            }
            switch (movimiento.TipoMovimiento)
            {
                case "E":
                    inventario.Stock -= result.Cantidad;
                    inventario.Stock += movimiento.Cantidad;
                    break;
                case "S":
                    inventario.Stock += result.Cantidad;
                    inventario.Stock -= movimiento.Cantidad;
                    break;
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
                var stocks = new InventariosController(_context);
                await stocks.PostInventario(new Inventario
                {
                    IdProducto = movimiento.IdProducto,
                    StockMinimo = 0,
                    Stock = 0,
                    IdTienda = movimiento.IdTienda,
                });
                inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == movimiento.IdTienda && p.IdProducto == movimiento.IdProducto);
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
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
                    if (inventario.Stock < 0)
                    {
                        return Problem("El stock el producto no puede ser negativo");
                    }
                    _context.Entry(inventario).State = EntityState.Modified;
                    try
                    {

                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        return Problem("Hubo un problema agregando el registro");
                    }
                    if (inventario.Stock < inventario.StockMinimo)
                    {
                        var correo = from i in _context.Tienda
                                     join u in _context.Usuarios on i.IdPropietario equals u.IntId
                                     where i.IntId == movimiento.IdTienda
                                     select u.Correo;
                        var nombreProducto = await (from p in _context.Productos
                                                    where p.IdProducto == movimiento.IdProducto
                                                    select p.Nombre).FirstOrDefaultAsync();

                        _loginService.EnviarCorreoAlerta(correo.ToString(), nombreProducto);

                    }


                    return CreatedAtAction("GetMovimiento", new { id = movimiento.IntId }, movimiento);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex);
                }
            }
           
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
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == movimiento.IdTienda && p.IdProducto == movimiento.IdProducto);
            if (movimiento == null)
            {
                return NotFound();
            }

            switch (movimiento.TipoMovimiento)
            {
                case "E":
                    inventario.Stock -= movimiento.Cantidad;
                    break;
                case "S":
                    inventario.Stock += movimiento.Cantidad;
                    break;
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
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(p => p.IdTienda == result.IdTienda && p.IdProducto == result.IdProducto);
            if (inventario == null)
            {
                return BadRequest();
                
            }
            var replaceOperation = personPatch.Operations.FirstOrDefault(op =>
        op.OperationType == OperationType.Replace && op.path.Equals("/Cantidad"));
            switch (result.TipoMovimiento)
            {
                case "E":
                    inventario.Stock -= result.Cantidad;
                    inventario.Stock += Convert.ToInt32(replaceOperation.value);
                    break;
                case "S":
                    inventario.Stock += result.Cantidad;
                    inventario.Stock -= Convert.ToInt32(replaceOperation.value);
                    break;
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
