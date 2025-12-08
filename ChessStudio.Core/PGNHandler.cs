using System.Text;

namespace ChessStudio.Core
{
    public class PGNHandler
    {
        public static string GeneratePGN(ChessBoard board, string whitePlayer = "Player 1", string blackPlayer = "Player 2", 
            string eventName = "Casual Game", string site = "Chess Studio", string? result = null)
        {
            StringBuilder pgn = new StringBuilder();

            // PGN headers
            pgn.AppendLine($"[Event \"{eventName}\"]");
            pgn.AppendLine($"[Site \"{site}\"]");
            pgn.AppendLine($"[Date \"{DateTime.Now:yyyy.MM.dd}\"]");
            pgn.AppendLine($"[Round \"-\"]");
            pgn.AppendLine($"[White \"{whitePlayer}\"]");
            pgn.AppendLine($"[Black \"{blackPlayer}\"]");

            // Determine result
            if (result == null)
            {
                result = board.State switch
                {
                    GameState.Checkmate => board.CurrentTurn == PieceColor.White ? "0-1" : "1-0",
                    GameState.Stalemate => "1/2-1/2",
                    GameState.Draw => "1/2-1/2",
                    _ => "*"
                };
            }

            pgn.AppendLine($"[Result \"{result}\"]");
            pgn.AppendLine();

            // Move list
            List<string> moves = board.MoveHistory;
            for (int i = 0; i < moves.Count; i++)
            {
                if (i % 2 == 0)
                {
                    pgn.Append($"{(i / 2) + 1}. ");
                }
                pgn.Append($"{moves[i]} ");

                if ((i + 1) % 12 == 0)
                {
                    pgn.AppendLine();
                }
            }

            pgn.AppendLine();
            pgn.AppendLine(result);

            return pgn.ToString();
        }

        public static void SavePGN(string pgn, string filepath)
        {
            File.WriteAllText(filepath, pgn);
        }

        public static string LoadPGN(string filepath)
        {
            return File.ReadAllText(filepath);
        }
    }
}
