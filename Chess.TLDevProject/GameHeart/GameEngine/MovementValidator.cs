using Chess.TLDevProject.GameHeart.Models;
using System.Drawing;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static class MovementValidator
    {
        public static bool IsMoveAllowed(LiveGameState state, MoveRecord move)
        {
            
            if (move.FromRow < 0 || move.FromRow > 7 || move.FromCol < 0 || move.FromCol > 7) 
                return false;
            if (move.ToRow < 0 || move.ToRow > 7 || move.ToCol < 0 || move.ToCol > 7) 
                return false;

            
            if (move.FromRow == move.ToRow && move.FromCol == move.ToCol) 
                return false;

            var piece = state.Board[move.FromRow, move.FromCol];


            if (piece == null) 
                return false;

            
            if (piece.PieceColor != state.SideToMove) 
                return false;

            var targetSquare = state.Board[move.ToRow, move.ToCol];
            

            if (targetSquare != null && targetSquare.PieceColor == piece.PieceColor) 
                return false;

            
            bool isItaValidMove = ValidateByPiece(state, move, piece);
            if (!isItaValidMove) 
                return false;

            
            bool isKingWillisCheckStatusIfPieceMoved = KingIsInCheckAfterMove(state, move, piece);
            if (isKingWillisCheckStatusIfPieceMoved) 
                return false;

            return true;
        }

        //// find the king on the borad to see if its incheck for game status
        public static bool IsKingInCheckNow(LiveGameState state, ChessPieceColor kingColor)
        {
            //cause 0 is used by other pices
            int kingRow = -1;
            int kingCol = -1;

            
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = state.Board[row, col];
                    if (piece != null &&  piece.PieceType == ChessPieceType.King &&  piece.PieceColor == kingColor)
                    {
                        kingRow = row;
                        kingCol = col;
                        break;
                    }
                }
                if (kingRow != -1) break; 
            }

            

            
            var dummyKing = new ChessPiece(kingColor, ChessPieceType.King);
            return IsSquareIsUnderAttack(state, dummyKing, kingRow, kingCol);
        }

        //====================================================================================
        //====================================================================================

        private static bool KingIsInCheckAfterMove(LiveGameState state, MoveRecord move, ChessPiece piece)
        {
            
            var tempState = new LiveGameState(new BoardSnapshot(state));

            
            tempState.Board[move.ToRow, move.ToCol] = tempState.Board[move.FromRow, move.FromCol];
            tempState.Board[move.FromRow, move.FromCol] = null;

            
            if (piece.PieceType == ChessPieceType.King && move.IsCastling)
            {
                int row = move.FromRow;
                if (move.ToCol == 6) // kingside
                {
                    tempState.Board[row, 5] = tempState.Board[row, 7];
                    tempState.Board[row, 7] = null;
                }
                else if (move.ToCol == 2) // queenside
                {
                    tempState.Board[row, 3] = tempState.Board[row, 0];
                    tempState.Board[row, 0] = null;
                }
            }

            int kingRow = -1, kingCol = -1;

            // find the king 
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var tempPiece = tempState.Board[row, col];
                    if (tempPiece != null && tempPiece.PieceType == ChessPieceType.King && tempPiece.PieceColor == piece.PieceColor)
                    {
                        kingRow = row;
                        kingCol = col;
                        break;
                    }
                }
            }

            if (kingRow == -1 || kingCol == -1)
                return false; // should never happen

            
            var dummyKing = new ChessPiece(piece.PieceColor, ChessPieceType.King);
            return IsSquareIsUnderAttack(tempState, dummyKing, kingRow, kingCol);
        }

        //====================================================================================
        //====================================================================================

        private static bool IsSquareIsUnderAttack(LiveGameState state,ChessPiece piece , int squareRow, int squareCol)
        {
            ChessPieceColor opponentColor = piece.PieceColor == ChessPieceColor.White
                ? ChessPieceColor.Black
                : ChessPieceColor.White;

            

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var PieceToTestIFCanAttack = state.Board[row, column];

                    if (PieceToTestIFCanAttack != null && PieceToTestIFCanAttack.PieceColor == opponentColor)
                    {
                        if (PieceToTestIFCanAttack.PieceType == ChessPieceType.Pawn)
                        {
                            int direction = (opponentColor == ChessPieceColor.White) ? -1 : 1;

                            // Pawns attack one row forward diagonally (depending on their color)
                            int attackRow = row + direction;
                            int leftAttackCol = column - 1;
                            int rightAttackCol = column + 1;

                            
                            if ((attackRow == squareRow && leftAttackCol == squareCol) ||
                                (attackRow == squareRow && rightAttackCol == squareCol))
                            {
                                return true;
                            }

                            
                            continue;
                        }

                        var moveToCheckIfPieceCanAttackSquare = new MoveRecord(row, column, squareRow, squareCol);
                        if (ValidateByPiece(state, moveToCheckIfPieceCanAttackSquare, PieceToTestIFCanAttack))
                        {
                            return true;
                        }
                        
                    }
                }
            }

            return false;
        }

        //====================================================================================
        //====================================================================================

        private static bool ValidateByPiece(LiveGameState state, MoveRecord move, ChessPiece piece)
        {
            if (piece.PieceType == ChessPieceType.Pawn)
            {
                return CheckIfPawnAllowedToMove(state, move, piece);
            }

            if (piece.PieceType == ChessPieceType.Knight)
            {
                return CheckIfKnightisAllowedToMove(state, move, piece);
            }

            if (piece.PieceType == ChessPieceType.King)
            {
                return CheckIfKingIsAllowedToMove(state, move, piece);
            }

            if (piece.PieceType == ChessPieceType.Rook)
            {
                return CheckIfRookIsAllowedToMove(state, move, piece);
            }


            if (piece.PieceType == ChessPieceType.Bishop)
            {
                return CheckIfBishopIsAllowedToMove(state, move, piece);
            }

            if (piece.PieceType == ChessPieceType.Queen)
            {
                return CheckIfQueenIsAllowedToMove(state, move, piece);
            }


            return false;
        }

        //====================================================================================
        //====================================================================================

        private static bool CheckIfPawnAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece pawn)
        {
            int fromRow = move.FromRow;
            int toRow = move.ToRow;
            int fromCol = move.FromCol;
            int toCol = move.ToCol;

            int movementDirectionValue;
            if (pawn.PieceColor == ChessPieceColor.White)
            {
                movementDirectionValue = -1;
            }
            else
            {
                movementDirectionValue = 1;
            }

            var theTargetSquare = state.Board[move.ToRow, move.ToCol];

            
            if ((toRow - fromRow == movementDirectionValue) && (toCol - fromCol == 0))
            {
                if (theTargetSquare != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //jump 2
            bool isPieceOnStillOnStartPoint = (pawn.PieceColor == ChessPieceColor.White && fromRow == 6) ||
                                              (pawn.PieceColor == ChessPieceColor.Black && fromRow == 1);
            if ((toRow - fromRow == movementDirectionValue * 2) && (toCol - fromCol == 0) && isPieceOnStillOnStartPoint)
            {
                int squareValuePawnJumpedRowValue = (pawn.PieceColor == ChessPieceColor.White) ? 5 : 2;

                if (state.Board[squareValuePawnJumpedRowValue, fromCol] != null)
                {
                    return false;
                }

                if (state.Board[toRow, toCol] != null)
                {
                    return false;
                }

                
                return true;
            }

            // diagonal capture and the enpassenet 

            if (move.ToRow - move.FromRow == movementDirectionValue && Math.Abs(move.ToCol - move.FromCol) == 1)
            {
                if (theTargetSquare != null && theTargetSquare.PieceColor != pawn.PieceColor)
                {
                    move.IsCapture = true;
                    return true;
                }

                if (theTargetSquare == null && state.EnPassantTarget.HasValue)
                {
                    if (state.EnPassantTarget == (move.ToRow, move.ToCol))
                    {
                        move.IsCapture = true;
                        move.IsEnPassant = true;
                        return true;
                    }
                    
                }

                return false;
            }

            return false;

        }

        //====================================================================================
        //====================================================================================

        private static bool CheckIfKnightisAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece knight)
        {
            int fromRow = move.FromRow;
            int toRow = move.ToRow;
            int fromCol = move.FromCol;
            int toCol = move.ToCol;
            var theTargetSquare = state.Board[move.ToRow, move.ToCol];


            if ((Math.Abs(toRow - fromRow) == 1 && Math.Abs(toCol - fromCol) == 2) ||
                (Math.Abs(toRow - fromRow) == 2 && Math.Abs(toCol - fromCol) == 1))
            {
                if (theTargetSquare == null)
                {
                    return true;
                }

                if (theTargetSquare != null && knight.PieceColor != theTargetSquare.PieceColor)
                {
                    move.IsCapture = true;
                    return true;
                }

            }

            return false;
        }


        //====================================================================================
        //====================================================================================

        private static bool CheckIfKingIsAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece king)
        {
            int fromRow = move.FromRow;
            int toRow = move.ToRow;
            int fromCol = move.FromCol;
            int toCol = move.ToCol;
            var theTargetSquare = state.Board[move.ToRow, move.ToCol];

            if (Math.Abs(toRow - fromRow) <= 1 && Math.Abs(toCol - fromCol) <= 1)
            {
                if (theTargetSquare == null)
                {
                    return true;
                }

                if (theTargetSquare != null && theTargetSquare.PieceColor != king.PieceColor)
                {
                    move.IsCapture = true;
                    return true;
                }

                
            }

            if (toRow == fromRow && Math.Abs(toCol - fromCol) == 2 && !king.HasMoved)
            {
                if (IsSquareIsUnderAttack(state, king, move.FromRow, move.FromCol))
                    return false;


                int rookRowValue = (king.PieceColor == ChessPieceColor.White) ? 7 : 0;
                int rookColValue;
                int squareValueKingJumpedColValue;

               
                if (fromCol < toCol)
                {
                    if (IsSquareIsUnderAttack(state, king, move.FromRow, move.FromCol + 1))
                        return false;

                    if (IsSquareIsUnderAttack(state, king, move.ToRow, move.ToCol))
                        return false;

                    
                    if (state.Board[fromRow, fromCol + 1] != null || state.Board[fromRow, fromCol + 2] != null)
                        return false;

                    squareValueKingJumpedColValue = fromCol + 1;
                    rookColValue = 7; 

                    var rook = state.Board[rookRowValue, rookColValue];
                    if (rook != null && !rook.HasMoved && rook.PieceType == ChessPieceType.Rook && rook.PieceColor == king.PieceColor)
                    {
                        if (theTargetSquare == null && state.Board[fromRow, squareValueKingJumpedColValue] == null)
                        {
                            move.IsCastling = true;
                            return true;
                        }
                    }
                }

                // queenside castling
                else if (fromCol > toCol)
                {
                    if (IsSquareIsUnderAttack(state, king, move.FromRow, move.FromCol - 1))
                        return false;

                    if (IsSquareIsUnderAttack(state, king, move.ToRow, move.ToCol))
                        return false;

                    
                    if (state.Board[fromRow, 1] != null || state.Board[fromRow, 2] != null || state.Board[fromRow, 3] != null)
                        return false;

                    squareValueKingJumpedColValue = fromCol - 1;
                    rookColValue = 0;

                    var rook = state.Board[rookRowValue, rookColValue];
                    if (rook != null && !rook.HasMoved && rook.PieceType == ChessPieceType.Rook && rook.PieceColor == king.PieceColor)
                    {
                        if (theTargetSquare == null && state.Board[fromRow, squareValueKingJumpedColValue] == null)
                        {
                            move.IsCastling = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        //====================================================================================
        //====================================================================================


        private static bool CheckIfRookIsAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece rook)
        {
            int fromRow = move.FromRow;
            int toRow = move.ToRow;
            int fromCol = move.FromCol;
            int toCol = move.ToCol;

            int DifferenceInRow = toRow - fromRow;
            int DiffereneceInCol = toCol - fromCol;

           
            if (DifferenceInRow != 0 && DiffereneceInCol != 0)
                return false;

           
            if (DifferenceInRow == 0)
            {
                int stepDirection = (DiffereneceInCol > 0) ? 1 : -1;

                for (int step = fromCol + stepDirection; step != toCol; step += stepDirection)
                {
                    if (state.Board[fromRow, step] != null)
                    {
                        return false; 
                    }
                }
            }

            
            else if (DiffereneceInCol == 0)
            {
                int stepDirection = (DifferenceInRow > 0) ? 1 : -1;

                for (int step = fromRow + stepDirection; step != toRow; step += stepDirection)
                {
                    if (state.Board[step, fromCol] != null)
                    {
                        return false; 
                    }
                }
            }

           
            var theTargetSquare = state.Board[toRow, toCol];

            if (theTargetSquare == null)
            {
                return true; 
            }

            if (theTargetSquare.PieceColor != rook.PieceColor)
            {
                move.IsCapture = true;
                return true; 
            }

            
            return false;
        }

        //====================================================================================
        //====================================================================================


        private static bool CheckIfBishopIsAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece bishop)
        {
            int fromRow = move.FromRow;
            int toRow = move.ToRow;
            int fromCol = move.FromCol;
            int toCol = move.ToCol;

            int DifferenceInRow = toRow - fromRow;
            int DiffereneceInCol = toCol - fromCol;

            if (Math.Abs(DifferenceInRow) != Math.Abs(DiffereneceInCol))
                return false;


            int rowStepDirection = (DifferenceInRow > 0) ? 1 : -1;
            int colStepDirection = (DiffereneceInCol > 0) ? 1 : -1;

            for (int step = 1; step < Math.Abs(DifferenceInRow); step++)
            {
                int squareRowToCheck = fromRow + step * rowStepDirection;
                int squareColToCheck = fromCol + step * colStepDirection;

                if (state.Board[squareRowToCheck, squareColToCheck] != null)
                {
                    
                    return false;
                }
            }
            var theTargetSquare = state.Board[toRow, toCol];

            if (theTargetSquare == null)
            {
                return true;
            }

            if (theTargetSquare.PieceColor != bishop.PieceColor)
            {
                move.IsCapture = true;
                return true; 
            }

            return false; 
        }

        //====================================================================================
        //====================================================================================


        private static bool CheckIfQueenIsAllowedToMove(LiveGameState state, MoveRecord move, ChessPiece queen)
        {
            bool canMoveLikeRook = CheckIfRookIsAllowedToMove(state, move, queen);
            bool canMoveLikeBishop = CheckIfBishopIsAllowedToMove(state, move, queen);

            if (canMoveLikeRook || canMoveLikeBishop)
            {
                return true;
            }

            return false;
        }

    }
}
