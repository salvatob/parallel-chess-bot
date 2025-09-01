namespace ChessBotCore;

internal static class FenParser {
    public static State ParseFen(string fen) {
        string[] tokens = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length != 6)
            throw new ArgumentException($"The {nameof(fen)} argument has incorrect" +
                                        $" number of parts. Should have 6");
        string pieces = tokens[0];
        string activeColor = tokens[1];
        string castles = tokens[2];
        string enpassant = tokens[3];
        string halfClock = tokens[4];
        string fullClock = tokens[5];


        State incompleteState = ParsePieces(pieces, State.Empty) with {
            WhiteIsActive = ParseActiveColor(activeColor),
            EnPassant = ParseEnpassant(enpassant),
            HalfMovesSincePawnMoveOrCapture = ParseClock(halfClock),
            FullMoves = ParseClock(fullClock)
        };

        State completeWithCastles = ParseCastles(castles, incompleteState);
        return completeWithCastles;
    }

    private static Bitboard ParseEnpassant(string enPassant) {
        if (enPassant == "-") return 0UL;

        Coordinates enPassantCoordinates = Coordinates.FromString(enPassant);
        // int indexCoord = enPassantCoordinates.To1D();
        return Bitboard.FromCoords(enPassantCoordinates);
        // return 1UL << indexCoord;
    }

    private static int ParseClock(string clock) {
        if (int.TryParse(clock, out int res) && res >= 0) return res;
        throw new ArgumentException("A clock part of parsed FEN string is in incorrect format");
    }

    private static State ParseCastles(string castlesString, State incompleteState) {
        bool wk = castlesString.Contains('K');
        bool wq = castlesString.Contains('Q');
        bool bk = castlesString.Contains('k');
        bool bq = castlesString.Contains('q');
        return incompleteState with {
            WhiteCastleKingSide = wk,
            WhiteCastleQueenSide = wq,
            BlackCastleKingSide = bk,
            BlackCastleQueenSide = bq
        };
    }

    private static State ParsePieces(string fenPieces, State incompleteState) {
        var board = FenToMatrix(fenPieces);

        return incompleteState with {
            WhiteQueens = ParsePiece(board, incompleteState.WhiteQueens, State.WhiteQueenSymbol),
            WhitePawns = ParsePiece(board, incompleteState.WhitePawns, State.WhitePawnSymbol),
            WhiteKing = ParsePiece(board, incompleteState.WhiteKing, State.WhiteKingSymbol),
            WhiteBishops = ParsePiece(board, incompleteState.WhiteBishops, State.WhiteBishopSymbol),
            WhiteKnights = ParsePiece(board, incompleteState.WhiteKnights, State.WhiteKnightSymbol),
            WhiteRooks = ParsePiece(board, incompleteState.WhiteRooks, State.WhiteRookSymbol),

            BlackQueens = ParsePiece(board, incompleteState.BlackQueens, State.BlackQueenSymbol),
            BlackPawns = ParsePiece(board, incompleteState.BlackPawns, State.BlackPawnSymbol),
            BlackKing = ParsePiece(board, incompleteState.BlackKing, State.BlackKingSymbol),
            BlackBishops = ParsePiece(board, incompleteState.BlackBishops, State.BlackBishopSymbol),
            BlackKnights = ParsePiece(board, incompleteState.BlackKnights, State.BlackKnightSymbol),
            BlackRooks = ParsePiece(board, incompleteState.BlackRooks, State.BlackRookSymbol)
        };
    }

    private static char[,] FenToMatrix(string fenPieces) {
        var board = new char[8, 8];
        var lines = fenPieces.Split('/');
        if (lines.Length != 8)
            throw new ArgumentException($"first part of a {nameof(fenPieces)} string must have 8" +
                                        $" lines of pieces but doesn't");
        for (var i = 0; i < lines.Length; i++) {
            var line = lines[7 - i];
            int offset = 0;
            foreach (char c in line) {
                if (int.TryParse(c.ToString(), out int emptyCells)) {
                    offset += emptyCells;
                    continue;
                }

                board[i, 7 - offset] = c;
                offset++;
            }
        }

        return board;
    }

    private static Bitboard ParsePiece(char[,] board, Bitboard pieceBits, char symbol) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                var cell = board[i, j];
                if (cell == symbol) SetBit(ref pieceBits, Coordinates.To1D(i, j));
            }
        }

        return pieceBits;
    }

    private static void SetBit(ref Bitboard bits, int index) {
        Bitboard mask = 1UL << index;
        bits |= mask;
    }

    private static bool ParseActiveColor(string color) {
        if (color == "w") return true;
        if (color == "b") return false;
        throw new ArgumentException($"The argument {nameof(color)} from FEN string is not in a correct format");
    }

    public static bool DetectActiveColor(string fen) {
        string[] tokens = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length != 6)
            throw new ArgumentException($"The {nameof(fen)} argument has incorrect" +
                                        $" number of parts. Should have 6");
        string activeColor = tokens[1];
        return ParseActiveColor(activeColor);
    }
    
}