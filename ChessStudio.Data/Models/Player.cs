namespace ChessStudio.Data.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Contact { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public List<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();
    }
}
