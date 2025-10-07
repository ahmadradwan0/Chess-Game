namespace Chess.TLDevProject.GameHeart.Models
{
    public class ChessPiece
    {
        public ChessPiece(ChessPieceColor pieceColor, ChessPieceType pieceType)
        {
            PieceColor = pieceColor;
            PieceType = pieceType;
            HasMoved = false;
        }

        public ChessPieceColor PieceColor { get; }

        public ChessPieceType PieceType { get; }
        public bool HasMoved { get; set; }

        public char ToFenChar()
        {
            return (PieceType, PieceColor) switch
            {
                (ChessPieceType.King, ChessPieceColor.White) => 'K',
                (ChessPieceType.Queen, ChessPieceColor.White) => 'Q',
                (ChessPieceType.Rook, ChessPieceColor.White) => 'R',
                (ChessPieceType.Bishop, ChessPieceColor.White) => 'B',
                (ChessPieceType.Knight, ChessPieceColor.White) => 'N',
                (ChessPieceType.Pawn, ChessPieceColor.White) => 'P',
                (ChessPieceType.King, ChessPieceColor.Black) => 'k',
                (ChessPieceType.Queen, ChessPieceColor.Black) => 'q',
                (ChessPieceType.Rook, ChessPieceColor.Black) => 'r',
                (ChessPieceType.Bishop, ChessPieceColor.Black) => 'b',
                (ChessPieceType.Knight, ChessPieceColor.Black) => 'n',
                (ChessPieceType.Pawn, ChessPieceColor.Black) => 'p',
                _ => throw new InvalidOperationException("Invalid piece.")
            };
        }

        public static ChessPiece FromFenChar(char c) => c switch
        {
            'K' => new ChessPiece(ChessPieceColor.White, ChessPieceType.King),
            'Q' => new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen),
            'R' => new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook),
            'B' => new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop),
            'N' => new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight),
            'P' => new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn),
            'k' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.King),
            'q' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen),
            'r' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook),
            'b' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop),
            'n' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight),
            'p' => new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn),
            _ => throw new ArgumentException($"Invalid FEN char: {c}")
        };
    }
}
