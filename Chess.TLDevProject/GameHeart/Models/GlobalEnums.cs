namespace Chess.TLDevProject.GameHeart.Models
{
    public enum ChessPieceColor
    {
        White = 0,
        Black = 1
    }

    public enum ChessPieceType
    {
        Pawn = 0,
        Rook = 1,
        Knight = 2,
        Bishop = 3,
        Queen = 4,
        King = 5
    }

    public enum ChessGameResult
    {
        None = 0,
        Draw = 1,
        WhiteWin = 2,
        BlackWin = 3,
        Resigned = 4,
        Aborted = 5
    }

    public enum ChessGameStatus
    {
        InProgress = 0,
        Check = 1,
        Checkmate = 2,
        Stalemate = 3,
        Ended = 4
    }

    public enum ChessPromotionPiece
    {
        Queen = 0,
        Rook = 1,
        Bishop = 2,
        Knight = 3
    }
}