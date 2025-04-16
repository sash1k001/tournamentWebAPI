namespace Turaev.WebAPI.Models
{
    public class TeamInMatch
    {
        public int Id { get; set; }
        public int TeamOneId { get; set; }
        public int? TeamTwoId { get; set; }
        public int MatchId { get; set; }
    }
}