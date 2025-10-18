using Chess.TLDevProject.GameHeart.Models;
using System.IO.Pipelines;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static class Movement
    {
        public static void Apply(LiveGameState state, MoveRecord move)
        {
            var piece = state.Board[move.FromRow, move.FromCol];
            if (piece == null)
                throw new InvalidOperationException("No piece square");


           
            if (piece.PieceType == ChessPieceType.King && move.IsCastling)
            {
                HandleCastlingToMoveCastle(state, move, piece);
            }

            else if (piece.PieceType == ChessPieceType.Pawn && move.IsEnPassant)
            {
                HandleEnPassantToRemovePawn(state, move, piece);
            }

            // Perform the move
            MovePiece(state, move, piece);


            // promote pawn when get to last raw
            if (piece.PieceType == ChessPieceType.Pawn)
            {
                bool isPawninLastRaw = (piece.PieceColor == ChessPieceColor.White && move.ToRow == 0) ||
                                       (piece.PieceColor == ChessPieceColor.Black && move.ToRow == 7);

                if (isPawninLastRaw)
                {
                    if (move.Promotion.HasValue)
                    {
                        state.Board[move.ToRow, move.ToCol] =
                            new ChessPiece(piece.PieceColor, move.Promotion!.Value);
                    }
                    else
                    {
                        move.Promotion = ChessPieceType.Queen;
                        state.Board[move.ToRow, move.ToCol] =
                            new ChessPiece(piece.PieceColor, move.Promotion!.Value);
                    }
                }
            }

            UpdateStateAndCountersAfterEveryMove(state, move, piece);



        }
        //====================================================================================

        private static void MovePiece(LiveGameState state, MoveRecord move, ChessPiece piece)
        {
            state.Board[move.ToRow, move.ToCol] = piece;
            state.Board[move.FromRow, move.FromCol] = null;
            piece.HasMoved = true;
        }

        private static void HandleEnPassantToRemovePawn(LiveGameState state, MoveRecord move, ChessPiece pawn)
        {
            int capturedRow = (pawn.PieceColor == ChessPieceColor.White)
                ? move.ToRow + 1
                : move.ToRow - 1;

            state.Board[capturedRow, move.ToCol] = null;
        }

        private static void HandleCastlingToMoveCastle(LiveGameState state, MoveRecord move, ChessPiece king)
        {

            int row = move.FromRow;

            if (move.ToCol == 6) 
            {
                var rook = state.Board[row, 7];
                state.Board[row, 5] = rook;
                state.Board[row, 7] = null;
                if (rook != null) rook.HasMoved = true;
            }
            else if (move.ToCol == 2) 
            {
                var rook = state.Board[row, 0];
                state.Board[row, 3] = rook;
                state.Board[row, 0] = null;
                if (rook != null) rook.HasMoved = true;
            }
        }

        private static void UpdateStateAndCountersAfterEveryMove(LiveGameState state, MoveRecord move, ChessPiece piece)
        {
            UpdateEnPassesntTarget(state, piece, move);
            UpdateCastlingRights(state, piece, move);
            UpdateMoveCounters(state, piece, move);
 
        }

        private static void UpdateMoveCounters(LiveGameState state, ChessPiece piece, MoveRecord move)
        {
            if (piece.PieceType == ChessPieceType.Pawn || move.IsCapture)
                state.HalfmoveClock = 0;
            else
                state.HalfmoveClock++;

            // Not used noww
            if (piece.PieceColor == ChessPieceColor.Black)
                state.FullmoveNumber++;

            
            state.MoveCounter++;

            
            state.SideToMove = (state.SideToMove == ChessPieceColor.White)
                ? ChessPieceColor.Black
                : ChessPieceColor.White;
        }

        private static void UpdateCastlingRights(LiveGameState state, ChessPiece piece, MoveRecord move)
        {
            if (piece.PieceType == ChessPieceType.King)
            {
                if (piece.PieceColor == ChessPieceColor.White)
                {
                    state.WhiteCanCastleKingside = false;
                    state.WhiteCanCastleQueenside = false;
                }
                else if(piece.PieceColor == ChessPieceColor.Black)
                {
                    state.BlackCanCastleKingside = false;
                    state.BlackCanCastleQueenside = false;
                }
            }
            else if(piece.PieceType == ChessPieceType.Rook)
            {
                if (piece.PieceColor == ChessPieceColor.White)
                {
                    if (move.FromRow == 7 && move.FromCol == 7) 
                        state.WhiteCanCastleKingside = false;

                    if (move.FromRow == 7 && move.FromCol == 0) 
                        state.WhiteCanCastleQueenside = false;
                }
                else if (piece.PieceColor == ChessPieceColor.Black)
                {
                    if (move.FromRow == 0 && move.FromCol == 7) 
                        state.BlackCanCastleKingside = false;
                   
                    if (move.FromRow == 0 && move.FromCol == 0) 
                        state.BlackCanCastleQueenside = false;

                }
            }
        }

        //if pawn moved 2 squares it will be a target for enpassent
        private static void UpdateEnPassesntTarget(LiveGameState state, ChessPiece piece, MoveRecord move)
        {
            
            state.EnPassantTarget = null;
            if (piece.PieceType == ChessPieceType.Pawn && Math.Abs(move.ToRow - move.FromRow) == 2)
            {
                int jumpedRow = (move.FromRow + move.ToRow) / 2;
                state.EnPassantTarget = (jumpedRow, move.FromCol);
            }
        }

    }
}
