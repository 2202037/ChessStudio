namespace ChessStudio.Core.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color, Position position) : base(PieceType.Pawn, color, position)
        {
        }

        public override List<Position> GetPossibleMoves(ChessBoard board)
        {
            List<Position> moves = new List<Position>();
            int direction = Color == PieceColor.White ? -1 : 1;

            // Forward move
            Position oneForward = new Position(Position.Row + direction, Position.Col);
            if (oneForward.IsValid() && board.GetPiece(oneForward) == null)
            {
                moves.Add(oneForward);

                // Two squares forward from starting position
                if (!HasMoved)
                {
                    Position twoForward = new Position(Position.Row + 2 * direction, Position.Col);
                    if (twoForward.IsValid() && board.GetPiece(twoForward) == null)
                    {
                        moves.Add(twoForward);
                    }
                }
            }

            // Diagonal captures
            int[] captureCols = { Position.Col - 1, Position.Col + 1 };
            foreach (int col in captureCols)
            {
                Position capturePos = new Position(Position.Row + direction, col);
                if (capturePos.IsValid())
                {
                    Piece? targetPiece = board.GetPiece(capturePos);
                    if (targetPiece != null && targetPiece.Color != Color)
                    {
                        moves.Add(capturePos);
                    }

                    // En passant
                    if (board.EnPassantTarget != null && capturePos.Equals(board.EnPassantTarget))
                    {
                        moves.Add(capturePos);
                    }
                }
            }

            return moves;
        }
    }
}
