namespace Chess.TLDevProject.GameHeart.DataBaseModels
{
    public class Match
    {
        public int MatchId { get; set; }            // Primary key
       
        public int UserId { get; set; }          // FK -> Users
        public byte Result { get; set; }                  // 0=None, 1=WhiteWin, 2=BlackWin, 3=Draw
        public byte Status { get; set; }                  // 1=InProgress, 2=Checkmate, 3=Stalemate, etc.

        public int TotalMoves { get; set; }
    }
}
