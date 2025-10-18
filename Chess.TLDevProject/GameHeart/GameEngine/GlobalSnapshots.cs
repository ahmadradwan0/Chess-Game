using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static class GlobalSnapshots
    {
        public static List<BoardSnapshot> Snapshots { get; } = new List<BoardSnapshot>();

        public static void Add(BoardSnapshot snapshot)
        {
            Snapshots.Add(snapshot);
        }

        public static BoardSnapshot? GetLast()
        {
            if (Snapshots.Count == 0)
                return null;
            return Snapshots[^1]; // last snapshot
        }

        public static bool RemoveLast()
        {
            if (Snapshots.Count == 0)
                return false;

            Snapshots.RemoveAt(Snapshots.Count - 1);
            return true;
        }
    }
}
