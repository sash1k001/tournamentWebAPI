using Microsoft.EntityFrameworkCore;
using Turaev.WebAPI.Models;

namespace Turaev.WebAPI.Data
{
    public class TuraevDbContext : DbContext
    {
        public TuraevDbContext(DbContextOptions<TuraevDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<UserInTeam> UsersInTeams { get; set; }
        public DbSet<TeamInMatch> TeamsInMatches { get; set; }
        public DbSet<TeamInTournament> TeamsInTournamnets { get; set; }
        public DbSet<Bracket> Brackets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=turaevdatabase.db");
        }
    }
}