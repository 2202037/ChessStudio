namespace ChessStudio.Core
{
    public abstract class Piece
    {
        public PieceType Type { get; protected set; }
        public PieceColor Color { get; protected set; }
        public Position Position { get; set; }
        public bool HasMoved { get; set; }

        protected Piece(PieceType type, PieceColor color, Position position)
        {
            Type = type;
            Color = color;
            Position = position;
            HasMoved = false;
        }

        public abstract List<Position> GetPossibleMoves(ChessBoard board);

        public virtual bool IsValidMove(Position to, ChessBoard board)
        {
            return GetPossibleMoves(board).Any(p => p.Equals(to));
        }

        protected bool IsPathClear(Position from, Position to, ChessBoard board)
        {
            int rowDir = Math.Sign(to.Row - from.Row);
            int colDir = Math.Sign(to.Col - from.Col);

            Position current = new Position(from.Row + rowDir, from.Col + colDir);

            while (!current.Equals(to))
            {
                if (board.GetPiece(current) != null)
                    return false;

                current = new Position(current.Row + rowDir, current.Col + colDir);
            }

            return true;
        }

        public char ToFENChar()
        {
            char symbol = Type switch
            {
                PieceType.King => 'k',
                PieceType.Queen => 'q',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                PieceType.Pawn => 'p',
                _ => '?'
            };

            return Color == PieceColor.White ? char.ToUpper(symbol) : symbol;
        }
    }
}
