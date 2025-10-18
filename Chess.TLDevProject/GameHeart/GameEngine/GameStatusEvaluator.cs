using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static class GameStatusEvaluator
    {
        public static void Evaluate(LiveGameState GameState, out ChessGameStatus status, out ChessGameResult result,
            out string message)
        {
            status = ChessGameStatus.InProgress;
            result = ChessGameResult.None;
            message = "";

            

            if (GameState.HalfmoveClock >= 100)
            {
                status = ChessGameStatus.Ended;
                result = ChessGameResult.Draw;
                message = "Draw : 50 Move Rule";
                return;
            }

            bool IsTheKingInCheck = MovementValidator.IsKingInCheckNow(GameState,GameState.SideToMove);

            var AllLegalMoves = MoveGenerator.GenerateAllLegalMoves(GameState, GameState.SideToMove);

            if (AllLegalMoves.Count == 0)
            {
                if (IsTheKingInCheck)
                {
                    status = ChessGameStatus.Checkmate;
                    result = GameState.SideToMove == ChessPieceColor.White
                        ? ChessGameResult.BlackWin
                        : ChessGameResult.WhiteWin;
                    message = "Checkmate";
                    
                }
                else
                {
                    status = ChessGameStatus.Stalemate;
                    result = ChessGameResult.Draw;
                    message = "Draw :  stalemate";

                }
                return;
            }

            if (IsTheKingInCheck)
            {
                status = ChessGameStatus.Check;
                result = ChessGameResult.None;
                message = "Check";
                return;
            }

            status = ChessGameStatus.InProgress;
            result = ChessGameResult.None;
            message = "Game continues";
        }
    }
}
