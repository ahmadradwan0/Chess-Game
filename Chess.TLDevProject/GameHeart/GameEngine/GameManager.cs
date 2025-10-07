using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public class GameManager
    {
        public LiveGameState GameState { get; private set; }

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

            CurrentGameBoardSnapshotsList.Add(new BoardSnapshot(GameState));

            Movement.Apply(GameState,move);

            CurrentGameMovesHistoryList.Add(move);

            return true;
        }

        public bool UndoMove()
        {
            if(CurrentGameBoardSnapshotsList.Count == 0)
            {
                return false;
            }

            var LastGameSnapShotSavedInList = CurrentGameBoardSnapshotsList[CurrentGameBoardSnapshotsList.Count - 1];

            GameState = new LiveGameState(LastGameSnapShotSavedInList);

            CurrentGameBoardSnapshotsList.RemoveAt(CurrentGameBoardSnapshotsList.Count - 1);
            CurrentGameMovesHistoryList.RemoveAt(CurrentGameMovesHistoryList.Count - 1);

            return true;
        }

        public void GameReset()
        {
            GameState = new LiveGameState();
            CurrentGameBoardSnapshotsList.Clear();
            CurrentGameMovesHistoryList.Clear();
            
        }


    }
}
