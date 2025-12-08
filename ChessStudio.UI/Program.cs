using ChessStudio.Core;
using ChessStudio.Core.Tournament;
using ChessStudio.Data;
using ChessStudio.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessStudio.UI
{
    class Program
    {
        static ChessStudioContext? dbContext;

        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║     Welcome to Chess Studio          ║");
            Console.WriteLine("║  Chess Gameplay & Tournament System  ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
            Console.WriteLine();

            InitializeDatabase();

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Main Menu ===");
                Console.WriteLine("1. Play Chess (Local Game)");
                Console.WriteLine("2. Tournament Management");
                Console.WriteLine("3. Player Management");
                Console.WriteLine("4. View Statistics");
                Console.WriteLine("5. Exit");
                Console.Write("\nSelect option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PlayChessGame();
                        break;
                    case "2":
                        TournamentMenu();
                        break;
                    case "3":
                        PlayerManagementMenu();
                        break;
                    case "4":
                        ViewStatistics();
                        break;
                    case "5":
                        running = false;
                        Console.WriteLine("\nThank you for using Chess Studio!");
                        break;
                    default:
                        Console.WriteLine("\nInvalid option. Please try again.");
                        break;
                }
            }
        }

        static void InitializeDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChessStudioContext>();
            optionsBuilder.UseSqlite("Data Source=chessstudio.db");
            dbContext = new ChessStudioContext(optionsBuilder.Options);
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Database initialized.");
        }

        static void PlayChessGame()
        {
            Console.WriteLine("\n=== Chess Game ===");
            Console.Write("White Player Name: ");
            string? whiteName = Console.ReadLine() ?? "White";
            Console.Write("Black Player Name: ");
            string? blackName = Console.ReadLine() ?? "Black";

            ChessBoard board = new ChessBoard();
            board.Display();

            while (board.State != GameState.Checkmate && board.State != GameState.Stalemate && board.State != GameState.Draw)
            {
                Console.Write($"\n{board.CurrentTurn}'s turn. Enter move (e.g., e2-e4) or 'quit': ");
                string? input = Console.ReadLine();

                if (input?.ToLower() == "quit")
                {
                    Console.WriteLine("Game ended by user.");
                    break;
                }

                if (string.IsNullOrEmpty(input) || !input.Contains('-'))
                {
                    Console.WriteLine("Invalid move format. Use format: e2-e4");
                    continue;
                }

                try
                {
                    string[] parts = input.Split('-');
                    Position from = Position.FromAlgebraic(parts[0].Trim());
                    Position to = Position.FromAlgebraic(parts[1].Trim());

                    if (board.MovePiece(from, to))
                    {
                        board.Display();
                        
                        if (board.State == GameState.Check)
                        {
                            Console.WriteLine("\n*** CHECK! ***");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid move! Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Game ended
            if (board.State == GameState.Checkmate)
            {
                PieceColor winner = board.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                Console.WriteLine($"\n*** CHECKMATE! {winner} wins! ***");
            }
            else if (board.State == GameState.Stalemate)
            {
                Console.WriteLine("\n*** STALEMATE! Game is a draw. ***");
            }
            else if (board.State == GameState.Draw)
            {
                Console.WriteLine("\n*** DRAW! (50-move rule) ***");
            }

            // Save PGN
            Console.Write("\nSave game? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                string pgn = PGNHandler.GeneratePGN(board, whiteName, blackName);
                string filename = $"game_{DateTime.Now:yyyyMMdd_HHmmss}.pgn";
                PGNHandler.SavePGN(pgn, filename);
                Console.WriteLine($"Game saved to {filename}");
            }
        }

        static void TournamentMenu()
        {
            Console.WriteLine("\n=== Tournament Management ===");
            Console.WriteLine("1. Create New Tournament");
            Console.WriteLine("2. View Tournaments");
            Console.WriteLine("3. Start Tournament Round");
            Console.WriteLine("4. Record Match Result");
            Console.WriteLine("5. View Standings");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateTournament();
                    break;
                case "2":
                    ViewTournaments();
                    break;
                case "3":
                    StartTournamentRound();
                    break;
                case "4":
                    RecordMatchResult();
                    break;
                case "5":
                    ViewStandings();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void CreateTournament()
        {
            if (dbContext == null) return;

            Console.WriteLine("\n=== Create New Tournament ===");
            Console.Write("Tournament Name: ");
            string? name = Console.ReadLine();
            
            Console.WriteLine("Format: 1. Swiss  2. Knockout  3. Round Robin");
            Console.Write("Select format: ");
            string? formatChoice = Console.ReadLine();
            
            TournamentFormat format = formatChoice switch
            {
                "1" => TournamentFormat.Swiss,
                "2" => TournamentFormat.Knockout,
                "3" => TournamentFormat.RoundRobin,
                _ => TournamentFormat.Swiss
            };

            Console.Write("Number of rounds: ");
            int rounds = int.TryParse(Console.ReadLine(), out int r) ? r : 5;

            var tournament = new Tournament
            {
                Name = name ?? "Unnamed Tournament",
                Format = format,
                Status = TournamentStatus.Pending,
                StartDate = DateTime.Now,
                NumberOfRounds = rounds
            };

            dbContext.Tournaments.Add(tournament);
            dbContext.SaveChanges();

            Console.WriteLine($"\nTournament '{tournament.Name}' created successfully! ID: {tournament.TournamentId}");
            
            // Register players
            Console.Write("Register players now? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                RegisterPlayersToTournament(tournament.TournamentId);
            }
        }

        static void RegisterPlayersToTournament(int tournamentId)
        {
            if (dbContext == null) return;

            var tournament = dbContext.Tournaments.Find(tournamentId);
            if (tournament == null)
            {
                Console.WriteLine("Tournament not found.");
                return;
            }

            var players = dbContext.Players.ToList();
            Console.WriteLine("\nAvailable Players:");
            foreach (var player in players)
            {
                Console.WriteLine($"{player.PlayerId}. {player.Name} (Rating: {player.Rating})");
            }

            Console.WriteLine("\nEnter player IDs to register (comma-separated):");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return;

            var playerIds = input.Split(',').Select(s => int.TryParse(s.Trim(), out int id) ? id : 0).Where(id => id > 0).ToList();

            foreach (var playerId in playerIds)
            {
                if (players.Any(p => p.PlayerId == playerId))
                {
                    dbContext.TournamentPlayers.Add(new TournamentPlayer
                    {
                        TournamentId = tournamentId,
                        PlayerId = playerId,
                        Score = 0,
                        Wins = 0,
                        Losses = 0,
                        Draws = 0
                    });
                }
            }

            dbContext.SaveChanges();
            Console.WriteLine($"\n{playerIds.Count} players registered to tournament.");
        }

        static void ViewTournaments()
        {
            if (dbContext == null) return;

            var tournaments = dbContext.Tournaments.ToList();
            Console.WriteLine("\n=== Tournaments ===");
            foreach (var tournament in tournaments)
            {
                Console.WriteLine($"ID: {tournament.TournamentId} | {tournament.Name} | {tournament.Format} | Status: {tournament.Status}");
            }
        }

        static void StartTournamentRound()
        {
            if (dbContext == null) return;

            Console.Write("Enter Tournament ID: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId)) return;

            var tournament = dbContext.Tournaments
                .Include(t => t.TournamentPlayers)
                .ThenInclude(tp => tp.Player)
                .Include(t => t.Rounds)
                .FirstOrDefault(t => t.TournamentId == tournamentId);

            if (tournament == null)
            {
                Console.WriteLine("Tournament not found.");
                return;
            }

            int nextRoundNumber = tournament.Rounds.Count + 1;
            if (nextRoundNumber > tournament.NumberOfRounds)
            {
                Console.WriteLine("All rounds completed.");
                return;
            }

            // Generate pairings using Swiss system
            var playerScores = tournament.TournamentPlayers.Select(tp => new PlayerScore
            {
                PlayerId = tp.PlayerId,
                PlayerName = tp.Player.Name,
                Score = tp.Score,
                OpponentIds = new List<int>(), // In production, track opponent history
                HasBye = false
            }).ToList();

            var pairings = SwissPairing.GeneratePairings(playerScores, nextRoundNumber);

            // Create round
            var round = new Round
            {
                TournamentId = tournamentId,
                RoundNumber = nextRoundNumber,
                StartTime = DateTime.Now,
                IsCompleted = false
            };

            dbContext.Rounds.Add(round);
            dbContext.SaveChanges();

            // Create matches
            foreach (var pairing in pairings)
            {
                dbContext.Matches.Add(new Match
                {
                    RoundId = round.RoundId,
                    WhitePlayerId = pairing.WhitePlayerId,
                    BlackPlayerId = pairing.BlackPlayerId,
                    Result = MatchResult.Pending,
                    StartTime = DateTime.Now
                });
            }

            dbContext.SaveChanges();

            Console.WriteLine($"\nRound {nextRoundNumber} started!");
            Console.WriteLine("Pairings:");
            foreach (var pairing in pairings)
            {
                if (pairing.IsBye)
                {
                    var player = tournament.TournamentPlayers.FirstOrDefault(tp => tp.PlayerId == pairing.WhitePlayerId);
                    Console.WriteLine($"  {player?.Player.Name} - BYE");
                }
                else
                {
                    var white = tournament.TournamentPlayers.FirstOrDefault(tp => tp.PlayerId == pairing.WhitePlayerId);
                    var black = tournament.TournamentPlayers.FirstOrDefault(tp => tp.PlayerId == pairing.BlackPlayerId);
                    Console.WriteLine($"  {white?.Player.Name} (White) vs {black?.Player.Name} (Black)");
                }
            }

            tournament.Status = TournamentStatus.InProgress;
            dbContext.SaveChanges();
        }

        static void RecordMatchResult()
        {
            if (dbContext == null) return;

            Console.Write("Enter Match ID: ");
            if (!int.TryParse(Console.ReadLine(), out int matchId)) return;

            var match = dbContext.Matches
                .Include(m => m.WhitePlayer)
                .Include(m => m.BlackPlayer)
                .FirstOrDefault(m => m.MatchId == matchId);

            if (match == null)
            {
                Console.WriteLine("Match not found.");
                return;
            }

            Console.WriteLine($"\nMatch: {match.WhitePlayer.Name} (White) vs {match.BlackPlayer.Name} (Black)");
            Console.WriteLine("Result: 1. White Win  2. Black Win  3. Draw");
            Console.Write("Enter result: ");

            string? choice = Console.ReadLine();
            MatchResult result = choice switch
            {
                "1" => MatchResult.WhiteWin,
                "2" => MatchResult.BlackWin,
                "3" => MatchResult.Draw,
                _ => MatchResult.Pending
            };

            match.Result = result;
            match.EndTime = DateTime.Now;

            // Update tournament player scores
            var round = dbContext.Rounds.Include(r => r.Tournament).FirstOrDefault(r => r.RoundId == match.RoundId);
            if (round != null)
            {
                var whiteTP = dbContext.TournamentPlayers
                    .FirstOrDefault(tp => tp.TournamentId == round.TournamentId && tp.PlayerId == match.WhitePlayerId);
                var blackTP = dbContext.TournamentPlayers
                    .FirstOrDefault(tp => tp.TournamentId == round.TournamentId && tp.PlayerId == match.BlackPlayerId);

                if (whiteTP != null && blackTP != null)
                {
                    if (result == MatchResult.WhiteWin)
                    {
                        whiteTP.Score += 1;
                        whiteTP.Wins++;
                        blackTP.Losses++;
                    }
                    else if (result == MatchResult.BlackWin)
                    {
                        blackTP.Score += 1;
                        blackTP.Wins++;
                        whiteTP.Losses++;
                    }
                    else if (result == MatchResult.Draw)
                    {
                        whiteTP.Score += 0.5;
                        blackTP.Score += 0.5;
                        whiteTP.Draws++;
                        blackTP.Draws++;
                    }
                }
            }

            dbContext.SaveChanges();
            Console.WriteLine("Match result recorded successfully!");
        }

        static void ViewStandings()
        {
            if (dbContext == null) return;

            Console.Write("Enter Tournament ID: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId)) return;

            var standings = dbContext.TournamentPlayers
                .Include(tp => tp.Player)
                .Where(tp => tp.TournamentId == tournamentId)
                .OrderByDescending(tp => tp.Score)
                .ThenByDescending(tp => tp.Wins)
                .ToList();

            Console.WriteLine("\n=== Tournament Standings ===");
            Console.WriteLine("Rank | Player Name           | Score | W | D | L");
            Console.WriteLine("-----|----------------------|-------|---|---|---");

            int rank = 1;
            foreach (var tp in standings)
            {
                Console.WriteLine($"{rank,4} | {tp.Player.Name,-20} | {tp.Score,5} | {tp.Wins,1} | {tp.Draws,1} | {tp.Losses,1}");
                rank++;
            }
        }

        static void PlayerManagementMenu()
        {
            Console.WriteLine("\n=== Player Management ===");
            Console.WriteLine("1. Register New Player");
            Console.WriteLine("2. View All Players");
            Console.WriteLine("3. Update Player Rating");
            Console.WriteLine("4. Back to Main Menu");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterPlayer();
                    break;
                case "2":
                    ViewPlayers();
                    break;
                case "3":
                    UpdatePlayerRating();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void RegisterPlayer()
        {
            if (dbContext == null) return;

            Console.WriteLine("\n=== Register New Player ===");
            Console.Write("Player Name: ");
            string? name = Console.ReadLine();
            
            Console.Write("Initial Rating (default 1200): ");
            int rating = int.TryParse(Console.ReadLine(), out int r) ? r : 1200;
            
            Console.Write("Contact (optional): ");
            string? contact = Console.ReadLine();

            var player = new Player
            {
                Name = name ?? "Unknown Player",
                Rating = rating,
                Contact = contact,
                RegistrationDate = DateTime.Now
            };

            dbContext.Players.Add(player);
            dbContext.SaveChanges();

            Console.WriteLine($"\nPlayer '{player.Name}' registered successfully! ID: {player.PlayerId}");
        }

        static void ViewPlayers()
        {
            if (dbContext == null) return;

            var players = dbContext.Players.OrderByDescending(p => p.Rating).ToList();
            Console.WriteLine("\n=== Registered Players ===");
            Console.WriteLine("ID   | Name                  | Rating | Contact");
            Console.WriteLine("-----|----------------------|--------|------------------");

            foreach (var player in players)
            {
                Console.WriteLine($"{player.PlayerId,4} | {player.Name,-20} | {player.Rating,6} | {player.Contact ?? "N/A"}");
            }
        }

        static void UpdatePlayerRating()
        {
            if (dbContext == null) return;

            Console.Write("Enter Player ID: ");
            if (!int.TryParse(Console.ReadLine(), out int playerId)) return;

            var player = dbContext.Players.Find(playerId);
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return;
            }

            Console.Write($"Current Rating: {player.Rating}. New Rating: ");
            if (int.TryParse(Console.ReadLine(), out int newRating))
            {
                player.Rating = newRating;
                dbContext.SaveChanges();
                Console.WriteLine("Rating updated successfully!");
            }
        }

        static void ViewStatistics()
        {
            if (dbContext == null) return;

            Console.WriteLine("\n=== System Statistics ===");
            
            int totalPlayers = dbContext.Players.Count();
            int totalTournaments = dbContext.Tournaments.Count();
            int completedTournaments = dbContext.Tournaments.Count(t => t.Status == TournamentStatus.Completed);
            int totalMatches = dbContext.Matches.Count();
            int completedMatches = dbContext.Matches.Count(m => m.Result != MatchResult.Pending);

            Console.WriteLine($"Total Players: {totalPlayers}");
            Console.WriteLine($"Total Tournaments: {totalTournaments}");
            Console.WriteLine($"Completed Tournaments: {completedTournaments}");
            Console.WriteLine($"Total Matches: {totalMatches}");
            Console.WriteLine($"Completed Matches: {completedMatches}");

            if (totalPlayers > 0)
            {
                var topPlayer = dbContext.Players.OrderByDescending(p => p.Rating).FirstOrDefault();
                if (topPlayer != null)
                {
                    Console.WriteLine($"\nTop Rated Player: {topPlayer.Name} ({topPlayer.Rating})");
                }
            }
        }
    }
}
