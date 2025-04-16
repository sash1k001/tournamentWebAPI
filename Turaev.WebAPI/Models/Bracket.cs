namespace Turaev.WebAPI.Models
{
    public class Bracket
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int Round { get; set; }
        public int MatchId { get; set; }
        public int? WinnerRoundId { get; set; }
    }
}