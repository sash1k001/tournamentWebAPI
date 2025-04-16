namespace Turaev.WebAPI.Models
{
    public class CreateTeamDto
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Game { get; set; }
        public int CaptainId { get; set; }
    }
}
