namespace Turaev.WebAPI.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WinnerId { get; set; } = -1;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
