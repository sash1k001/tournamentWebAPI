using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using Turaev.WebAPI.Data;
using Turaev.WebAPI.Models;

namespace Turaev.WebAPI.Controllers
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly TuraevDbContext _context;

        public TeamController(TuraevDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<Team>> GetAllTeam()
        {
            return Ok(_context.Teams.ToList());
        }

        [HttpGet("{teamId}/roster")]
        public ActionResult<IEnumerable<UserInTeam>> GetRosterTeam(int teamId)
        {
            return Ok(_context.UsersInTeams.Where(t => t.TeamId == teamId).ToList());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
        {
            var team = new Team { Name = dto.Name, Tag = dto.Tag, Game = dto.Game, CaptainId = dto.CaptainId };
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            var addedCapToTeam = new UserInTeam { TeamId = team.Id, UserId = team.CaptainId };

            _context.UsersInTeams.Add(addedCapToTeam);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Команда создана", team});
        }

        [HttpPost("{teamId}/add-user")]
        public async Task<IActionResult> AddUserToTeam(int teamId, [FromBody] UserToTeamDto dto)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound("Команда не найдена.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("Пользователь не найден.");

            var userInTeam = await _context.UsersInTeams.FirstOrDefaultAsync(t => t.UserId == dto.UserId);
            if (userInTeam != null)
                return BadRequest("Пользователь состоит в команде.");

            var addedUser = new UserInTeam { TeamId = team.Id, UserId = user.Id };

            _context.UsersInTeams.Add(addedUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь добавлен в команду", team.Name });
        }

        [HttpDelete("remove-user/{userId}")]
        public async Task<IActionResult> RemoveUserFromTeam(int userId)
        {
            var user = await _context.UsersInTeams.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return BadRequest("Пользователь не состоит в команде.");

            _context.UsersInTeams.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Пользователь был успешно удалён из вашей команды.");
        }

        [HttpDelete("remove/{teamId}")]
        public async Task<IActionResult> RemoveTeam(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                return NotFound("Такой команды не существует.");

            var userInTeam = await _context.UsersInTeams.FirstOrDefaultAsync(t => t.TeamId == teamId);
            if (userInTeam != null)
                return BadRequest("В команде состоят пользователи.");

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok("Команда успешно удалена.");
        }
    }
}