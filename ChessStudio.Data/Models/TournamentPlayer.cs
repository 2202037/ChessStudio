namespace ChessStudio.Data.Models
{
    public class TournamentPlayer
    {
        public int TournamentPlayerId { get; set; }
        public int TournamentId { get; set; }
        public int PlayerId { get; set; }
        public double Score { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        // Navigation properties
        public Tournament Tournament { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
