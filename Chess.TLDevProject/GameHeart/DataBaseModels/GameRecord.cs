namespace Chess.TLDevProject.GameHeart.DataBaseModels
{
    public class GameRecord
    {
        public int GameRecordId { get; set; }
        public int MatchId { get; set; }

        public byte FromRow { get; set; }
        public byte ToRow { get; set; }
        public byte FromCol { get; set; }
        public byte ToCol { get; set; }

        public bool IsCapture { get; set; }
        public bool IsCastling { get; set; }
        public bool IsEnPassant { get; set; }

        public byte? Promotion { get; set; }

        public string FenString { get; set; } = "";

        public int MoveNumber { get; set; }
    }
}
