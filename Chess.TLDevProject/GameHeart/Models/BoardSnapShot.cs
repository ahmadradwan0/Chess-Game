using Chess.TLDevProject.GameHeart.GameEngine;

namespace Chess.TLDevProject.GameHeart.Models
{
    public class BoardSnapshot
    {
        public ChessPiece?[,] Board { get; } = new ChessPiece?[8, 8];
        public ChessPieceColor SideToMove { get; init; } = ChessPieceColor.White;
        public bool WhiteCanCastleKingside { get; init; } = true;
        public bool WhiteCanCastleQueenside { get; init; } = true;
        public bool BlackCanCastleKingside { get; init; } = true;
        public bool BlackCanCastleQueenside { get; init; } = true;
        public (int Row, int Col)? EnPassantTarget { get; init; }

        // Optional FEN counters
        public int HalfmoveClock { get; init; } = 0;
        public int FullmoveNumber { get; init; } = 1;

        // ✅ Constructor version
        public BoardSnapshot(LiveGameState state)
        {
            // Assuming state.Board is a ChessPiece?[,] and other fields match
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Board[row, col] = state.Board[row, col];
                }
            }

            SideToMove = state.SideToMove;
            WhiteCanCastleKingside = state.WhiteCanCastleKingside;
            WhiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            BlackCanCastleKingside = state.BlackCanCastleKingside;
            BlackCanCastleQueenside = state.BlackCanCastleQueenside;
            EnPassantTarget = state.EnPassantTarget;

            HalfmoveClock = state.HalfmoveClock;
            FullmoveNumber = state.FullmoveNumber;
        }

        // ✅ Static wrapper for readability or future flexibility
        public static BoardSnapshot CreateFrom(LiveGameState state)
        {
            return new BoardSnapshot(state);
        }
    }
}
