using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Turaev.WebAPI.Data;
using Turaev.WebAPI.Models;

namespace Turaev.WebAPI.Controllers
{
    [Route("api/tournament")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly TuraevDbContext _context;

        public TournamentController(TuraevDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentCreateDto dto)
        {
            var tournament = new Tournament
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Турнир успешно создан" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTournament()
        {
            var tournaments = await _context.Tournaments.ToListAsync();
            return Ok(tournaments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournamentById(int id)
        {
            var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
                return NotFound("Турнир не найден.");

            return Ok(tournament);
        }

        [HttpGet("{tournamentId}/members")]
        public ActionResult<IEnumerable<TeamInTournament>> GetTournamentMembersById(int tournamentId)
        {
            var members = _context.TeamsInTournamnets.Where(t => t.TournamentId == tournamentId).ToList();
            return Ok(members);
        }

        [HttpPost("{tournamentId}/add-team/{teamId}")]
        public async Task<IActionResult> AddTeamToTournament(int tournamentId, int teamId)
        {
            var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
            if (tournament == null)
                return NotFound("Турнир не найден.");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                return NotFound("Команда не найдена.");

            var teamInTournament = await _context.TeamsInTournamnets.FirstOrDefaultAsync(t => t.TournamentId == tournamentId && t.TeamId == teamId);
            
            if (teamInTournament != null)
            {
                if (teamInTournament.TournamentId == tournamentId && teamInTournament.TeamId == teamId)
                    return BadRequest("Команда уже участвует в турнире.");
            }

                var dto = new TeamInTournament
                {
                    TeamId = teamId,
                    TournamentId = tournamentId
                };

            _context.TeamsInTournamnets.Add(dto);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Команда успешно добавлена в турнир", tournament});
        }

        [HttpDelete("{tournamentId}/remove-team/{teamId}")]
        public async Task<IActionResult> RemoveTeamToTournament(int tournamentId, int teamId)
        {
            var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
            if (tournament == null)
                return NotFound("Турнир не найден.");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                return NotFound("Команда не найдена.");

            var teamInTournament = await _context.TeamsInTournamnets.FirstOrDefaultAsync(t => t.TournamentId == tournamentId && t.TeamId == teamId);

            if (teamInTournament == null)
            {
                return BadRequest("Команда не состоит в этом турнире.");
            }

            var dto = await _context.TeamsInTournamnets.FindAsync(teamInTournament.Id);

            _context.TeamsInTournamnets.Remove(dto);
            await _context.SaveChangesAsync();
            return Ok(new {message = "Команда успешно исключена из турнира."});
        }

        [HttpPost("{tournamentId}/generate-bracket")]
        public async Task<IActionResult> GenerateBracket(int tournamentId)
        {
            var teams = _context.TeamsInTournamnets
                .Where(t => t.TournamentId == tournamentId)
                .Select(t => t.TeamId)
                .ToList();

            if (teams.Count < 2)
            {
                return BadRequest("Недостаточно команд для создания турнира.");
            }

            var matches = new List<Match>();
            var brackets = new List<Bracket>();
            int round = 1;

            while (teams.Count > 1)
            {
                var nextRoundTeams = new List<int>();

                for (int i = 0; i < teams.Count; i += 2)
                {
                    int teamOneId = teams[i];
                    int? teamTwoId = (i + 1 < teams.Count) ? teams[i + 1] : null;

                    var match = new Match
                    {
                        TournamentId = tournamentId,
                        MatchDate = DateTime.UtcNow.AddDays(round), 
                        Status = "Scheduled",
                        ScoreTeamOne = 0,
                        ScoreTeamTwo = 0
                    };

                    _context.Matches.Add(match);
                    await _context.SaveChangesAsync();


                    _context.TeamsInMatches.Add(new TeamInMatch
                    {
                        MatchId = match.Id,
                        TeamOneId = teamOneId,
                        TeamTwoId = teamTwoId
                    });
                    await _context.SaveChangesAsync();


                    var bracket = new Bracket
                    {
                        TournamentId = tournamentId,
                        Round = round,
                        MatchId = match.Id,
                        WinnerRoundId = null 
                    };
                    _context.Brackets.Add(bracket);
                    await _context.SaveChangesAsync();
                }

                break;
            }

            await _context.SaveChangesAsync();
            return Ok("Турнирная сетка успешно создана.");
        }

        [HttpPost("{matchId}/set-result")]
        public async Task<IActionResult> SetMatchResult(int matchId, [FromBody] MatchResultDto result)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
            {
                return NotFound("Матч не найден.");
            }

            match.ScoreTeamOne = result.ScoreTeamOne;
            match.ScoreTeamTwo = result.ScoreTeamTwo;
            match.Status = "Completed";
            await _context.SaveChangesAsync();

            int winnerId = result.ScoreTeamOne > result.ScoreTeamTwo ? result.TeamOneId : result.TeamTwoId;

            var bracket = await _context.Brackets.FirstOrDefaultAsync(b => b.MatchId == matchId);
            if (bracket != null)
            {
                bracket.WinnerRoundId = winnerId;
                await _context.SaveChangesAsync();
            }

            var currentRoundBrackets = await _context.Brackets
                .Where(b => b.TournamentId == match.TournamentId && b.Round == bracket.Round)
                .ToListAsync();

            if (currentRoundBrackets.All(b => b.WinnerRoundId != null))
            {
                int nextRound = bracket.Round + 1;
                var winners = currentRoundBrackets.Select(b => b.WinnerRoundId.Value).ToList();

                if (winners.Count == 1)
                {
                    var tournament = await _context.Tournaments.FindAsync(match.TournamentId);
                    if (tournament != null)
                    {
                        tournament.WinnerId = winners.First();
                        tournament.EndDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                    return Ok("Турнир завершен. Победитель: " + winners.First());
                }

                for (int i = 0; i < winners.Count; i += 2)
                {
                    int teamOneId = winners[i];
                    int? teamTwoId = (i + 1 < winners.Count) ? winners[i + 1] : null;

                    var newMatch = new Match
                    {
                        TournamentId = match.TournamentId,
                        MatchDate = DateTime.UtcNow.AddDays(nextRound),
                        Status = "Scheduled",
                        ScoreTeamOne = 0,
                        ScoreTeamTwo = 0
                    };

                    _context.Matches.Add(newMatch);
                    await _context.SaveChangesAsync();

                    _context.TeamsInMatches.Add(new TeamInMatch
                    {
                        MatchId = newMatch.Id,
                        TeamOneId = teamOneId,
                        TeamTwoId = teamTwoId
                    });
                    await _context.SaveChangesAsync();

                    var newBracket = new Bracket
                    {
                        TournamentId = match.TournamentId,
                        Round = nextRound,
                        MatchId = newMatch.Id,
                        WinnerRoundId = null
                    };
                    _context.Brackets.Add(newBracket);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok("Результат матча сохранен и победитель продвинут.");
        }
    }
}