namespace ChessStudio.Core.Pieces
{
    public class King : Piece
    {
        public King(PieceColor color, Position position) : base(PieceType.King, color, position)
        {
        }

        public override List<Position> GetPossibleMoves(ChessBoard board)
        {
            List<Position> moves = new List<Position>();
            int[] directions = { -1, 0, 1 };

            foreach (int dRow in directions)
            {
                foreach (int dCol in directions)
                {
                    if (dRow == 0 && dCol == 0) continue;

                    Position newPos = new Position(Position.Row + dRow, Position.Col + dCol);
                    if (newPos.IsValid())
                    {
                        Piece? targetPiece = board.GetPiece(newPos);
                        if (targetPiece == null || targetPiece.Color != Color)
                        {
                            moves.Add(newPos);
                        }
                    }
                }
            }

            // Castling
            if (!HasMoved && !board.IsKingInCheck(Color))
            {
                // Kingside castling
                Position kingsideRookPos = new Position(Position.Row, 7);
                Piece? kingsideRook = board.GetPiece(kingsideRookPos);
                if (kingsideRook != null && !kingsideRook.HasMoved && kingsideRook.Type == PieceType.Rook)
                {
                    if (IsPathClear(Position, kingsideRookPos, board))
                    {
                        Position castlePos = new Position(Position.Row, Position.Col + 2);
                        moves.Add(castlePos);
                    }
                }

                // Queenside castling
                Position queensideRookPos = new Position(Position.Row, 0);
                Piece? queensideRook = board.GetPiece(queensideRookPos);
                if (queensideRook != null && !queensideRook.HasMoved && queensideRook.Type == PieceType.Rook)
                {
                    if (IsPathClear(Position, queensideRookPos, board))
                    {
                        Position castlePos = new Position(Position.Row, Position.Col - 2);
                        moves.Add(castlePos);
                    }
                }
            }

            return moves;
        }
    }
}
