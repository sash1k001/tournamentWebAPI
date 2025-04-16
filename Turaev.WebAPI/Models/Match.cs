namespace Turaev.WebAPI.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public DateTime? MatchDate { get; set; }
        public string Status { get; set; }
        public int ScoreTeamOne { get; set; }
        public int ScoreTeamTwo { get; set; }
    }
}