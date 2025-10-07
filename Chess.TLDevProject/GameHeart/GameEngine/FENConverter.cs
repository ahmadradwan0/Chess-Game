using Chess.TLDevProject.GameHeart.Models;

namespace Chess.TLDevProject.GameHeart.GameEngine
{
    public static class FENConverter
    {
        public static string ExportFromObjToFenString(LiveGameState gameState)
        {
            string fenCharacters = "";

            for (int row = 0; row < 8; row++)
            {
                int EmptySquares = 0;

                for (int col = 0; col < 8; col++)
                {
                    var piece = gameState.Board[row, col];
                    if (piece != null)
                    {
                        if (EmptySquares > 0)
                        {
                            fenCharacters += EmptySquares.ToString();
                            EmptySquares = 0;
                        }

                        fenCharacters += piece.ToFenChar();
                    }
                    // if piece in null
                    else
                    {
                        EmptySquares++;
                    }
                }

                if (EmptySquares > 0)
                {
                    fenCharacters += EmptySquares.ToString();
                }

                if (row < 7)
                {
                    fenCharacters += '/';
                }
            }

            fenCharacters += " " + (gameState.SideToMove == ChessPieceColor.White ? "w" : "b");

            string castling = "";
            if (gameState.WhiteCanCastleKingside) castling += "K";
            if (gameState.WhiteCanCastleQueenside) castling += "Q";
            if (gameState.BlackCanCastleKingside) castling += "k";
            if (gameState.BlackCanCastleQueenside) castling += "q";
            if (string.IsNullOrEmpty(castling)) castling = "-";

            fenCharacters += " " + castling;
            return fenCharacters;
        }

        public static LiveGameState ExportFromFenStringToObj(string fenString)
        {
            var state = new LiveGameState();

            var fenStringParts = fenString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string boardPiecesPart = fenStringParts[0];
            string sideToMovePart = fenStringParts[1];
            string castlingRightsPart = fenStringParts[2];

            var rowsInBoard = boardPiecesPart.Split('/');

            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char cr in rowsInBoard[row])
                {
                    if (char.IsDigit(cr))
                    {
                        int squaresToJump = (int)char.GetNumericValue(cr);
                        for (int square =0 ; square < squaresToJump; square++)
                        {
                            state.Board[row, col] = null;
                            col++;

                        }
                    }
                    // if character not number
                    else
                    {
                        state.Board[row,col] = ChessPiece.FromFenChar(cr);
                        col++;
                    }
                }
            }

            state.SideToMove = (sideToMovePart == "w") ? ChessPieceColor.White : ChessPieceColor.Black;
            state.WhiteCanCastleKingside = castlingRightsPart.Contains('K');
            state.WhiteCanCastleQueenside = castlingRightsPart.Contains('Q');
            state.BlackCanCastleKingside = castlingRightsPart.Contains('k');
            state.BlackCanCastleQueenside = castlingRightsPart.Contains('q');

            return state;
        }

    }
}
