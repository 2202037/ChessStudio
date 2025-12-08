namespace ChessStudio.Core.Tournament
{
    public class PlayerScore
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public double Score { get; set; }
        public List<int> OpponentIds { get; set; } = new List<int>();
        public bool HasBye { get; set; }
    }

    public class Pairing
    {
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public bool IsBye { get; set; }
    }

    public class SwissPairing
    {
        public static List<Pairing> GeneratePairings(List<PlayerScore> players, int roundNumber)
        {
            List<Pairing> pairings = new List<Pairing>();

            // Sort players by score (descending)
            var sortedPlayers = players.OrderByDescending(p => p.Score).ToList();

            // Track players who have been paired
            HashSet<int> pairedPlayers = new HashSet<int>();

            // Try to pair players with similar scores
            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                if (pairedPlayers.Contains(sortedPlayers[i].PlayerId))
                    continue;

                PlayerScore player1 = sortedPlayers[i];
                PlayerScore? player2 = null;

                // Find a suitable opponent
                for (int j = i + 1; j < sortedPlayers.Count; j++)
                {
                    if (pairedPlayers.Contains(sortedPlayers[j].PlayerId))
                        continue;

                    PlayerScore candidate = sortedPlayers[j];

                    // Check if they have already played each other
                    if (!player1.OpponentIds.Contains(candidate.PlayerId))
                    {
                        player2 = candidate;
                        break;
                    }
                }

                if (player2 != null)
                {
                    // Alternate colors based on round number
                    bool player1IsWhite = (i % 2 == 0) == (roundNumber % 2 == 1);

                    pairings.Add(new Pairing
                    {
                        WhitePlayerId = player1IsWhite ? player1.PlayerId : player2.PlayerId,
                        BlackPlayerId = player1IsWhite ? player2.PlayerId : player1.PlayerId,
                        IsBye = false
                    });

                    pairedPlayers.Add(player1.PlayerId);
                    pairedPlayers.Add(player2.PlayerId);
                }
                else
                {
                    // Odd player out gets a bye
                    if (!player1.HasBye)
                    {
                        pairings.Add(new Pairing
                        {
                            WhitePlayerId = player1.PlayerId,
                            BlackPlayerId = 0,
                            IsBye = true
                        });

                        pairedPlayers.Add(player1.PlayerId);
                    }
                }
            }

            return pairings;
        }

        public static List<PlayerScore> CalculateStandings(List<PlayerScore> players)
        {
            return players.OrderByDescending(p => p.Score)
                         .ThenByDescending(p => p.OpponentIds.Count)
                         .ToList();
        }
    }
}
