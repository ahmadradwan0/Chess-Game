namespace Chess.TLDevProject.GameHeart.Models
{
    public class MoveRecord
    {
        public MoveRecord(
            int fromRow,
            int fromCol,
            int toRow,
            int toCol,
            bool isCapture = false,
            bool isCastling = false,
            bool isEnPassant = false,
            ChessPieceType? promotion = null)
        {
            FromRow = fromRow;
            FromCol = fromCol;
            ToRow = toRow;
            ToCol = toCol;
            IsCapture = isCapture;
            IsCastling = isCastling;
            IsEnPassant = isEnPassant;
            Promotion = promotion;
        }

        public int FromRow { get; }
        public int FromCol { get; }
        public int ToRow { get; }
        public int ToCol { get; }

        public bool IsCapture { get; set; } = false;
        public bool IsCastling { get; set; } = false;
        public bool IsEnPassant { get; set; } = false;
        public ChessPieceType? Promotion { get; set; } = null;

        public static string CoordsInNumbersToSquareNotation(int row, int col)
        {
            char ColLetter = (char)('a' + col);   // 0..7 -> a..h
            int rowNumber = 8 - row;             // 0..7 -> 8..1
            return $"{ColLetter}{rowNumber}";
        }

        /*
         *
            Row → 0  1  2  3  4  5  6  7
           Col ↓
           0     a8 b8 c8 d8 e8 f8 g8 h8
           1     a7 b7 c7 d7 e7 f7 g7 h7
           2     a6 b6 c6 d6 e6 f6 g6 h6
           3     a5 b5 c5 d5 e5 f5 g5 h5
           4     a4 b4 c4 d4 e4 f4 g4 h4
           5     a3 b3 c3 d3 e3 f3 g3 h3
           6     a2 b2 c2 d2 e2 f2 g2 h2
           7     a1 b1 c1 d1 e1 f1 g1 h1
         */


    }
}
