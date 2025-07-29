using Digesett.Server.Data;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Digesett.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController(AppDbContext context) : ControllerBase
    {
        public AppDbContext Context { get; set; } = context;

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetAllContacts()
        {
            var contacts = await Context.Contacts.ToListAsync();
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<ActionResult> AddContact(Contact Contacto) 
        {
            Contacto.Active = true;
            await Context.Contacts.AddAsync(Contacto);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id) 
        {
            var contact = await Context.Contacts.FindAsync(id);
            if (contact is null) 
            {
                return NotFound("Contacto no encontrado...");
            }
            return Ok(contact);
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<Contact>> UpdateContacts(int id, Contact editarcontacto) 
        {
            var contactdb = await Context.Contacts.FindAsync(id);
            if (contactdb is null) 
            {
                return NotFound("contacto a modificar no encontrdo");
            }
            contactdb.Name = editarcontacto.Name;
            contactdb.Departamento = editarcontacto.Departamento;
            contactdb.Cargo = editarcontacto.Cargo;
            contactdb.Correo = editarcontacto.Correo;
            contactdb.Active = editarcontacto.Active;
            await Context.SaveChangesAsync();
            return Ok(contactdb);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteContacts(int id) 
        {
            var item = await Context.Contacts.FindAsync(id);
            if (item is null) 
            {
                return NotFound("contacto no encontrado...");
            }
            Context.Remove(item);
            await Context.SaveChangesAsync();
            return Ok(item);
        }
    }
}
