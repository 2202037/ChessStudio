namespace ChessStudio.Core.Pieces
{
    public class Rook : Piece
    {
        public Rook(PieceColor color, Position position) : base(PieceType.Rook, color, position)
        {
        }

        public override List<Position> GetPossibleMoves(ChessBoard board)
        {
            List<Position> moves = new List<Position>();

            int[][] directions = new int[][]
            {
                new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 }
            };

            foreach (int[] direction in directions)
            {
                for (int i = 1; i < 8; i++)
                {
                    Position newPos = new Position(Position.Row + direction[0] * i, Position.Col + direction[1] * i);
                    if (!newPos.IsValid()) break;

                    Piece? targetPiece = board.GetPiece(newPos);
                    if (targetPiece == null)
                    {
                        moves.Add(newPos);
                    }
                    else
                    {
                        if (targetPiece.Color != Color)
                            moves.Add(newPos);
                        break;
                    }
                }
            }

            return moves;
        }
    }
}
