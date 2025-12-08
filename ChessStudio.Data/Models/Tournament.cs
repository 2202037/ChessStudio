namespace ChessStudio.Data.Models
{
    public enum TournamentFormat
    {
        Swiss,
        Knockout,
        RoundRobin
    }

    public enum TournamentStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public class Tournament
    {
        public int TournamentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TournamentFormat Format { get; set; }
        public TournamentStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int NumberOfRounds { get; set; }

        // Navigation properties
        public List<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();
        public List<Round> Rounds { get; set; } = new List<Round>();
    }
}
