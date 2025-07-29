using Digesett.Server.Data;
using Digesett.Shared.DTO;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digesett.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(AppDbContext context) : ControllerBase
    {
        public readonly AppDbContext Context = context;

        [HttpPost]
        public async Task<ActionResult> AddUsers(User user)
        {
            user.Active = true;
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await Context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUsersById(int id)
        {
            var user = await Context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound("usuario no encontrado...");
            }
            return Ok(user);
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<User>> UpdateUsers(int id, User user)
        {
            var userdb = await Context.Users.FindAsync(id);
            if (userdb is null)
            {
                return NotFound("user no encontrado...");
            }
            userdb.Name = user.Name;
            userdb.Departament = user.Departament;
            userdb.Cargo = user.Cargo;
            userdb.Email = user.Email;
            userdb.Phone = user.Phone;
            userdb.Password = user.Password;
            userdb.TypeUser = user.TypeUser;
            userdb.Active = user.Active;
            await Context.SaveChangesAsync();
            return Ok(userdb);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteUsers(int id)
        {
            var item = await Context.Users.FindAsync(id);
            if (item is null)
            {
                return NotFound("user no encontrado...");
            }
            Context.Remove(item);
            await Context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPost("/Api/Users/login")]
        
        public async Task<ActionResult<bool>> LoginUsers(LoginDTO login) 
        {
            var user = await Context.Users.Where(x => x.Email == login.Email).FirstOrDefaultAsync();
            if (user is not null)
            {
                if (user.Password == login.Password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }
    }
}
