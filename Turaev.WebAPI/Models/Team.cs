namespace Turaev.WebAPI.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Game { get; set; }
        public int CaptainId { get; set; }
    }
}