namespace Turaev.WebAPI.Models
{
    public class TournamentCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}