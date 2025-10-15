using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public class LiveGameState
    {
        public ChessPiece?[,] Board { get; set; } = new ChessPiece?[8, 8];

        public ChessPieceColor SideToMove { get; set; } = ChessPieceColor.White;

       
        public bool WhiteCanCastleKingside { get; set; } = true;
        public bool WhiteCanCastleQueenside { get; set; } = true;
        public bool BlackCanCastleKingside { get; set; } = true;
        public bool BlackCanCastleQueenside { get; set; } = true;

        
        public (int Row, int Col)? EnPassantTarget { get; set; }

        
        public int HalfmoveClock { get; set; } = 0;

        // not really useful now 
        public int FullmoveNumber { get; set; } = 1;

        public int MoveCounter { get; set; } = 0;

        // filling initail pieces
        public LiveGameState()
        {
            // White pieces
            Board[7, 0] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook);
            Board[7, 1] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight);
            Board[7, 2] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop);
            Board[7, 3] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen);
            Board[7, 4] = new ChessPiece(ChessPieceColor.White, ChessPieceType.King);
            Board[7, 5] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop);
            Board[7, 6] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight);
            Board[7, 7] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook);
            for (int col = 0; col < 8; col++)
                Board[6, col] = new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn);

            // Black pieces
            Board[0, 0] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook);
            Board[0, 1] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight);
            Board[0, 2] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop);
            Board[0, 3] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen);
            Board[0, 4] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King);
            Board[0, 5] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop);
            Board[0, 6] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight);
            Board[0, 7] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook);
            for (int col = 0; col < 8; col++)
                Board[1, col] = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn);
        }

        public LiveGameState(BoardSnapshot snapshot)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Board[row, col] = snapshot.Board[row, col];
                }
            }

            SideToMove = snapshot.SideToMove;
            WhiteCanCastleKingside = snapshot.WhiteCanCastleKingside;
            WhiteCanCastleQueenside = snapshot.WhiteCanCastleQueenside;
            BlackCanCastleKingside = snapshot.BlackCanCastleKingside;
            BlackCanCastleQueenside = snapshot.BlackCanCastleQueenside;
            EnPassantTarget = snapshot.EnPassantTarget;
            HalfmoveClock = snapshot.HalfmoveClock;
            FullmoveNumber = snapshot.FullmoveNumber;

        }

    }

     
}
