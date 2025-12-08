namespace ChessStudio.Data.Models
{
    public class Round
    {
        public int RoundId { get; set; }
        public int TournamentId { get; set; }
        public int RoundNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        public Tournament Tournament { get; set; } = null!;
        public List<Match> Matches { get; set; } = new List<Match>();
    }
}
