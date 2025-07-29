using Digesett.Server.Data;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digesett.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivoController(AppDbContext context) : ControllerBase
    {
        public AppDbContext Context { get; set; } = context;


        [HttpPost]
        public async Task<ActionResult> AddActivo(ActivoFijo activo) 
        {
            activo.Active = true;
            await Context.Activos.AddAsync(activo);
            await Context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ActivoFijo>>> GetActivosFijosAll() 
        {
            var activos = await Context.Activos.ToListAsync();
            return Ok(activos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivoFijo>> GetActivoById(int id) 
       {
            var activos = await Context.Activos.FindAsync(id);
            if (activos is null) 
            {
                return NotFound("activo no encontrado");
            }
            return Ok(activos);
       }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<ActivoFijo>> UpdateActivoFijos(int id, ActivoFijo activo) 
        {
            var activodb = await Context.Activos.FindAsync(id);
            if (activodb is null) 
            {
                return NotFound("activo no encontado");
            }
            activodb.Name = activo.Name;
            activo.Category = activo.Category;
            activodb.Active = activo.Active;
            await Context.SaveChangesAsync();
            return Ok(activodb);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteActivos(int id)
        {
            var item = await Context.Activos.FindAsync(id);
            if (item is null)
            {
                return NotFound("activo no encontrado...");
            }
            Context.Remove(item);
            await Context.SaveChangesAsync();
            return Ok(item);
        }
    }
}



        




