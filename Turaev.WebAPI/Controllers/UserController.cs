using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turaev.WebAPI.Data;
using Turaev.WebAPI.Models;

namespace Turaev.WebAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TuraevDbContext _context;

        public UserController(TuraevDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto request)
        {
            if (request == null)
                return BadRequest("Данные не предоставлены.");

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Пользователь не найден.");

            user.Name = request.Name ?? user.Name;
            user.Email = request.Email ?? user.Email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Данные пользователя обновлены успешно."});
        }
    }
}
