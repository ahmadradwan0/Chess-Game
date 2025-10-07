using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static  class MoveGenerator
    {
        public static List<MoveRecord> GenerateAllLegalMoves(LiveGameState state, ChessPieceColor color)
        {
            var legalMoves = new List<MoveRecord>();

            for (int fromRow = 0; fromRow < 8; fromRow++)
            {
                for (int fromCol = 0; fromCol < 8; fromCol++)
                {
                    var piece = state.Board[fromRow, fromCol];
                    if (piece == null || piece.PieceColor != color)
                        continue;

                    // Try every square on the board as a target
                    for (int toRow = 0; toRow < 8; toRow++)
                    {
                        for (int toCol = 0; toCol < 8; toCol++)
                        {
                            if (fromRow == toRow && fromCol == toCol)
                                continue;

                            var move = new MoveRecord(fromRow, fromCol, toRow, toCol);

                            if (MovementValidator.IsMoveAllowed(state, move))
                            {
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
            }

            return legalMoves;
        }
    }
}
