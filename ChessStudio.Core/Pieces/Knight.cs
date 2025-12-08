namespace ChessStudio.Core.Pieces
{
    public class Knight : Piece
    {
        public Knight(PieceColor color, Position position) : base(PieceType.Knight, color, position)
        {
        }

        public override List<Position> GetPossibleMoves(ChessBoard board)
        {
            List<Position> moves = new List<Position>();

            int[][] knightMoves = new int[][]
            {
                new int[] { -2, -1 }, new int[] { -2, 1 }, new int[] { -1, -2 }, new int[] { -1, 2 },
                new int[] { 1, -2 }, new int[] { 1, 2 }, new int[] { 2, -1 }, new int[] { 2, 1 }
            };

            foreach (int[] move in knightMoves)
            {
                Position newPos = new Position(Position.Row + move[0], Position.Col + move[1]);
                if (newPos.IsValid())
                {
                    Piece? targetPiece = board.GetPiece(newPos);
                    if (targetPiece == null || targetPiece.Color != Color)
                    {
                        moves.Add(newPos);
                    }
                }
            }

            return moves;
        }
    }
}
