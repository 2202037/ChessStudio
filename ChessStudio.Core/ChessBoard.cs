using ChessStudio.Core.Pieces;

namespace ChessStudio.Core
{
    public enum GameState
    {
        Active,
        Check,
        Checkmate,
        Stalemate,
        Draw
    }

    public class ChessBoard
    {
        private Piece?[,] board;
        public PieceColor CurrentTurn { get; private set; }
        public GameState State { get; private set; }
        public Position? EnPassantTarget { get; private set; }
        public int HalfMoveClock { get; private set; }
        public int FullMoveNumber { get; private set; }
        public List<string> MoveHistory { get; private set; }

        private const int FIFTY_MOVE_RULE_LIMIT = 100; // 100 half-moves = 50 full moves

        public ChessBoard()
        {
            board = new Piece?[8, 8];
            CurrentTurn = PieceColor.White;
            State = GameState.Active;
            EnPassantTarget = null;
            HalfMoveClock = 0;
            FullMoveNumber = 1;
            MoveHistory = new List<string>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Place pawns
            for (int col = 0; col < 8; col++)
            {
                board[1, col] = new Pawn(PieceColor.Black, new Position(1, col));
                board[6, col] = new Pawn(PieceColor.White, new Position(6, col));
            }

            // Place rooks
            board[0, 0] = new Rook(PieceColor.Black, new Position(0, 0));
            board[0, 7] = new Rook(PieceColor.Black, new Position(0, 7));
            board[7, 0] = new Rook(PieceColor.White, new Position(7, 0));
            board[7, 7] = new Rook(PieceColor.White, new Position(7, 7));

            // Place knights
            board[0, 1] = new Knight(PieceColor.Black, new Position(0, 1));
            board[0, 6] = new Knight(PieceColor.Black, new Position(0, 6));
            board[7, 1] = new Knight(PieceColor.White, new Position(7, 1));
            board[7, 6] = new Knight(PieceColor.White, new Position(7, 6));

            // Place bishops
            board[0, 2] = new Bishop(PieceColor.Black, new Position(0, 2));
            board[0, 5] = new Bishop(PieceColor.Black, new Position(0, 5));
            board[7, 2] = new Bishop(PieceColor.White, new Position(7, 2));
            board[7, 5] = new Bishop(PieceColor.White, new Position(7, 5));

            // Place queens
            board[0, 3] = new Queen(PieceColor.Black, new Position(0, 3));
            board[7, 3] = new Queen(PieceColor.White, new Position(7, 3));

            // Place kings
            board[0, 4] = new King(PieceColor.Black, new Position(0, 4));
            board[7, 4] = new King(PieceColor.White, new Position(7, 4));
        }

        public Piece? GetPiece(Position pos)
        {
            if (!pos.IsValid()) return null;
            return board[pos.Row, pos.Col];
        }

        public bool MovePiece(Position from, Position to)
        {
            Piece? piece = GetPiece(from);
            if (piece == null || piece.Color != CurrentTurn)
                return false;

            // Special handling for castling
            if (piece.Type == PieceType.King && !piece.HasMoved && Math.Abs(to.Col - from.Col) == 2)
            {
                return HandleCastling(from, to);
            }

            if (!piece.IsValidMove(to, this))
                return false;

            // Check if move puts own king in check
            if (WouldKingBeInCheck(from, to, CurrentTurn))
                return false;

            return ExecuteMove(from, to);
        }

        private bool HandleCastling(Position from, Position to)
        {
            // Check if king is in check
            if (IsKingInCheck(CurrentTurn))
                return false;

            // Kingside castling
            if (to.Col > from.Col)
            {
                Position rookPos = new Position(from.Row, 7);
                Piece? rook = GetPiece(rookPos);
                if (rook == null || rook.HasMoved || rook.Type != PieceType.Rook)
                    return false;

                // Check if path is clear
                for (int col = from.Col + 1; col < rookPos.Col; col++)
                {
                    if (GetPiece(new Position(from.Row, col)) != null)
                        return false;
                }

                // Check if king passes through check
                Position intermediatePos = new Position(from.Row, from.Col + 1);
                if (WouldKingBeInCheck(from, intermediatePos, CurrentTurn))
                    return false;

                // Check if destination is in check
                if (WouldKingBeInCheck(from, to, CurrentTurn))
                    return false;
            }
            // Queenside castling
            else
            {
                Position rookPos = new Position(from.Row, 0);
                Piece? rook = GetPiece(rookPos);
                if (rook == null || rook.HasMoved || rook.Type != PieceType.Rook)
                    return false;

                // Check if path is clear
                for (int col = rookPos.Col + 1; col < from.Col; col++)
                {
                    if (GetPiece(new Position(from.Row, col)) != null)
                        return false;
                }

                // Check if king passes through check
                Position intermediatePos = new Position(from.Row, from.Col - 1);
                if (WouldKingBeInCheck(from, intermediatePos, CurrentTurn))
                    return false;

                // Check if destination is in check
                if (WouldKingBeInCheck(from, to, CurrentTurn))
                    return false;
            }

            return ExecuteMove(from, to);
        }

        private bool ExecuteMove(Position from, Position to)
        {
            Piece? piece = GetPiece(from);
            if (piece == null) return false;

            Piece? capturedPiece = GetPiece(to);
            bool isCapture = capturedPiece != null;
            bool isPawnMove = piece.Type == PieceType.Pawn;

            // Handle castling
            if (piece.Type == PieceType.King && Math.Abs(to.Col - from.Col) == 2)
            {
                // Kingside castling
                if (to.Col > from.Col)
                {
                    Piece? rook = GetPiece(new Position(from.Row, 7));
                    if (rook != null)
                    {
                        board[from.Row, 5] = rook;
                        rook.Position = new Position(from.Row, 5);
                        board[from.Row, 7] = null;
                        rook.HasMoved = true;
                    }
                }
                // Queenside castling
                else
                {
                    Piece? rook = GetPiece(new Position(from.Row, 0));
                    if (rook != null)
                    {
                        board[from.Row, 3] = rook;
                        rook.Position = new Position(from.Row, 3);
                        board[from.Row, 0] = null;
                        rook.HasMoved = true;
                    }
                }
            }

            // Handle en passant capture
            if (piece.Type == PieceType.Pawn && EnPassantTarget != null && to.Equals(EnPassantTarget))
            {
                int captureRow = CurrentTurn == PieceColor.White ? to.Row + 1 : to.Row - 1;
                board[captureRow, to.Col] = null;
                isCapture = true;
            }

            // Update en passant target
            EnPassantTarget = null;
            if (piece.Type == PieceType.Pawn && Math.Abs(to.Row - from.Row) == 2)
            {
                int epRow = CurrentTurn == PieceColor.White ? from.Row - 1 : from.Row + 1;
                EnPassantTarget = new Position(epRow, from.Col);
            }

            // Move the piece
            board[to.Row, to.Col] = piece;
            board[from.Row, from.Col] = null;
            piece.Position = to;
            piece.HasMoved = true;

            // Handle pawn promotion
            if (piece.Type == PieceType.Pawn && (to.Row == 0 || to.Row == 7))
            {
                board[to.Row, to.Col] = new Queen(piece.Color, to);
            }

            // Update half move clock
            if (isPawnMove || isCapture)
                HalfMoveClock = 0;
            else
                HalfMoveClock++;

            // Record move
            string moveNotation = $"{from.ToAlgebraic()}-{to.ToAlgebraic()}";
            MoveHistory.Add(moveNotation);

            // Switch turn
            CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            if (CurrentTurn == PieceColor.White)
                FullMoveNumber++;

            // Update game state
            UpdateGameState();

            return true;
        }

        public bool IsKingInCheck(PieceColor color)
        {
            Position? kingPos = FindKing(color);
            if (kingPos == null) return false;

            PieceColor opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece? piece = board[row, col];
                    if (piece != null && piece.Color == opponentColor)
                    {
                        if (piece.GetPossibleMoves(this).Any(p => p.Equals(kingPos)))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool WouldKingBeInCheck(Position from, Position to, PieceColor color)
        {
            // Create a temporary board state
            Piece? piece = GetPiece(from);
            Piece? captured = GetPiece(to);

            if (piece == null) return true;

            // Temporarily make the move
            board[to.Row, to.Col] = piece;
            board[from.Row, from.Col] = null;
            Position originalPos = piece.Position;
            piece.Position = to;

            bool inCheck = IsKingInCheck(color);

            // Restore the board
            board[from.Row, from.Col] = piece;
            board[to.Row, to.Col] = captured;
            piece.Position = originalPos;

            return inCheck;
        }

        private Position? FindKing(PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece? piece = board[row, col];
                    if (piece != null && piece.Type == PieceType.King && piece.Color == color)
                    {
                        return new Position(row, col);
                    }
                }
            }
            return null;
        }

        private void UpdateGameState()
        {
            bool hasLegalMoves = HasAnyLegalMoves(CurrentTurn);

            if (IsKingInCheck(CurrentTurn))
            {
                State = hasLegalMoves ? GameState.Check : GameState.Checkmate;
            }
            else
            {
                State = hasLegalMoves ? GameState.Active : GameState.Stalemate;
            }

            // Check for draw by 50-move rule
            if (HalfMoveClock >= FIFTY_MOVE_RULE_LIMIT)
            {
                State = GameState.Draw;
            }
        }

        private bool HasAnyLegalMoves(PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece? piece = board[row, col];
                    if (piece != null && piece.Color == color)
                    {
                        List<Position> moves = piece.GetPossibleMoves(this);
                        foreach (Position move in moves)
                        {
                            if (!WouldKingBeInCheck(piece.Position, move, color))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public string ToFEN()
        {
            string fen = "";

            // Board position
            for (int row = 0; row < 8; row++)
            {
                int emptyCount = 0;
                for (int col = 0; col < 8; col++)
                {
                    Piece? piece = board[row, col];
                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen += emptyCount;
                            emptyCount = 0;
                        }
                        fen += piece.ToFENChar();
                    }
                }
                if (emptyCount > 0)
                    fen += emptyCount;
                if (row < 7)
                    fen += "/";
            }

            // Active color
            fen += CurrentTurn == PieceColor.White ? " w " : " b ";

            // Castling availability (simplified)
            fen += "- ";

            // En passant target
            fen += EnPassantTarget != null ? EnPassantTarget.ToAlgebraic() : "-";

            // Half move clock
            fen += $" {HalfMoveClock}";

            // Full move number
            fen += $" {FullMoveNumber}";

            return fen;
        }

        public void Display()
        {
            Console.WriteLine("\n   a b c d e f g h");
            Console.WriteLine("  ┌───────────────┐");

            for (int row = 0; row < 8; row++)
            {
                Console.Write($"{8 - row} │");
                for (int col = 0; col < 8; col++)
                {
                    Piece? piece = board[row, col];
                    if (piece == null)
                    {
                        Console.Write(" .");
                    }
                    else
                    {
                        string symbol = piece.Type switch
                        {
                            PieceType.King => "K",
                            PieceType.Queen => "Q",
                            PieceType.Rook => "R",
                            PieceType.Bishop => "B",
                            PieceType.Knight => "N",
                            PieceType.Pawn => "P",
                            _ => "?"
                        };

                        symbol = piece.Color == PieceColor.White ? symbol : symbol.ToLower();
                        Console.Write($" {symbol}");
                    }
                }
                Console.WriteLine($" │ {8 - row}");
            }

            Console.WriteLine("  └───────────────┘");
            Console.WriteLine("   a b c d e f g h");
            Console.WriteLine($"\nTurn: {CurrentTurn}");
            Console.WriteLine($"State: {State}");
        }
    }
}
