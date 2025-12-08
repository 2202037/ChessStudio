namespace ChessStudio.Core
{
    public class Position
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool IsValid()
        {
            return Row >= 0 && Row < 8 && Col >= 0 && Col < 8;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Position other)
            {
                return Row == other.Row && Col == other.Col;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public string ToAlgebraic()
        {
            char file = (char)('a' + Col);
            int rank = 8 - Row;
            return $"{file}{rank}";
        }

        public static Position FromAlgebraic(string notation)
        {
            if (notation.Length != 2)
                throw new ArgumentException("Invalid algebraic notation");

            char file = notation[0];
            char rank = notation[1];

            int col = file - 'a';
            int row = 8 - (rank - '0');

            return new Position(row, col);
        }
    }
}
