using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public class GameManager
    {
        public LiveGameState GameState { get; set; }

        public List<MoveRecord> CurrentGameMovesHistoryList { get; private set; }
        public List<BoardSnapshot> CurrentGameBoardSnapshotsList { get; private set; }
        public GameManager()
        {
            GameState = new LiveGameState();
            CurrentGameBoardSnapshotsList = new List<BoardSnapshot>();
            CurrentGameMovesHistoryList = new List<MoveRecord>();
        }

        public bool MakeAMove(MoveRecord move)
        {
            var PieceToMove = GameState.Board[move.FromRow, move.FromCol];

            if (PieceToMove == null)
            {
                return false;
            }

            if (!MovementValidator.IsMoveAllowed(GameState, move))
            {
                return false;
            }

            GlobalSnapshots.Add(new BoardSnapshot(GameState));

            Movement.Apply(GameState,move);

            CurrentGameMovesHistoryList.Add(move);

            GameStatusEvaluator.Evaluate(GameState, out var status, out var result, out var message);


            return true;
        }

        public bool UndoMove()
        {
            var snapshot = GlobalSnapshots.GetLast();
            if (snapshot == null)
                return false;

            GameState = new LiveGameState(snapshot);
            GlobalSnapshots.RemoveLast();
            return true;
        }



    }
}
