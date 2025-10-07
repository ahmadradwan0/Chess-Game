namespace Chess.TLDevProject.GameHeart.Models
{
    public enum ChessPieceColor
    {
        White,Black
    }

    public enum ChessPieceType
    {
        Pawn,Rook,Knight,Bishop,Queen,King
    }

    public enum ChessGameResult
    {
        Draw,WhiteWin,BlackWin, Resigned, Aborted
    }

    public enum ChessGameStatus
    {
        NotStarted, InProgress, Check, Checkmate, Stalemate,Ended
    }
    public enum ChessPromotionPiece
    {
        Queen, Rook, Bishop, Knight
    }
}
