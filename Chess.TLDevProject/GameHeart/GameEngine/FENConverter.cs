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

            // side to move
            fenCharacters += " " + (gameState.SideToMove == ChessPieceColor.White ? "w" : "b");

            // castling rights
            string castling = "";
            if (gameState.WhiteCanCastleKingside) castling += "K";
            if (gameState.WhiteCanCastleQueenside) castling += "Q";
            if (gameState.BlackCanCastleKingside) castling += "k";
            if (gameState.BlackCanCastleQueenside) castling += "q";
            if (string.IsNullOrEmpty(castling)) castling = "-";

            fenCharacters += " " + castling;

            
            fenCharacters += " ";
            if (gameState.EnPassantTarget.HasValue)
            {
                var (row, col) = gameState.EnPassantTarget.Value;
                fenCharacters += $"{row}{col}"; 
            }
            else
            {
                fenCharacters += "-";
            }

            
            fenCharacters += $" {gameState.HalfmoveClock} {gameState.FullmoveNumber}";

            return fenCharacters;
        }

        public static LiveGameState ExportFromFenStringToObj(string fenString)
        {
            var state = new LiveGameState();

            var fenStringParts = fenString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string boardPiecesPart = fenStringParts[0];
            string sideToMovePart = fenStringParts.Length > 1 ? fenStringParts[1] : "w";     // ✅ safety
            string castlingRightsPart = fenStringParts.Length > 2 ? fenStringParts[2] : "-"; // ✅ safety
            string enPassantPart = fenStringParts.Length > 3 ? fenStringParts[3] : "-";      // ✅ safety
            string halfmovePart = fenStringParts.Length > 4 ? fenStringParts[4] : "0";       // ✅ safety
            string fullmovePart = fenStringParts.Length > 5 ? fenStringParts[5] : "1";       // ✅ safety

            var rowsInBoard = boardPiecesPart.Split('/');

            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char cr in rowsInBoard[row])
                {
                    if (char.IsDigit(cr))
                    {
                        int squaresToJump = (int)char.GetNumericValue(cr);
                        for (int square = 0; square < squaresToJump; square++)
                        {
                            state.Board[row, col] = null;
                            col++;
                        }
                    }
                    else
                    {
                        state.Board[row, col] = ChessPiece.FromFenChar(cr);
                        col++;
                    }
                }
            }

            // side to move
            state.SideToMove = (sideToMovePart == "w") ? ChessPieceColor.White : ChessPieceColor.Black;
            state.WhiteCanCastleKingside = castlingRightsPart.Contains('K');
            state.WhiteCanCastleQueenside = castlingRightsPart.Contains('Q');
            state.BlackCanCastleKingside = castlingRightsPart.Contains('k');
            state.BlackCanCastleQueenside = castlingRightsPart.Contains('q');

            //  Parse en passant target from "rowcol" string
            if (enPassantPart != "-" && enPassantPart.Length == 2)
            {
                if (char.IsDigit(enPassantPart[0]) && char.IsDigit(enPassantPart[1]))
                {
                    int row = int.Parse(enPassantPart[0].ToString());
                    int col = int.Parse(enPassantPart[1].ToString());
                    state.EnPassantTarget = (row, col);
                }
            }

            // parse halfmove and fullmove counters
            if (int.TryParse(halfmovePart, out int half))
                state.HalfmoveClock = half;
            if (int.TryParse(fullmovePart, out int full))
                state.FullmoveNumber = full;

            return state;
        }
    }
}
