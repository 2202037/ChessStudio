namespace ChessStudio.Data.Models
{
    public enum MatchResult
    {
        Pending,
        WhiteWin,
        BlackWin,
        Draw
    }

    public class Match
    {
        public int MatchId { get; set; }
        public int RoundId { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public MatchResult Result { get; set; }
        public string? PGN { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // Navigation properties
        public Round Round { get; set; } = null!;
        public Player WhitePlayer { get; set; } = null!;
        public Player BlackPlayer { get; set; } = null!;
    }
}
