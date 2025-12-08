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

            // Note: Castling is handled separately in the MovePiece method
            // to avoid infinite recursion with IsKingInCheck

            return moves;
        }
    }
}
