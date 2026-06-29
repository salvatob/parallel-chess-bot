namespace ChessBotCore;

internal class FenParser {
    private readonly State _state;
    private FenParser() {
        _state = State.Empty;
    }
    
    public static State ParseFen(string fen) {
        var parser = new FenParser();
        return parser.Parse(fen);
    }

    private State Parse(string fen) {
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

        ParsePieces(pieces);
        _state.WhiteIsActive = ParseActiveColor(activeColor);
        _state.EnPassant = ParseEnpassant(enpassant);
        _state.HalfMovesSincePawnMoveOrCapture = ParseClock(halfClock);
        _state.FullMoves = ParseClock(fullClock);
        ParseCastles(castles);

        return _state;
    }

    private static Bitboard ParseEnpassant(string enPassant) {
        if (enPassant == "-") return 0UL;

        Coordinates enPassantCoordinates = Coordinates.FromString(enPassant);
        return Bitboard.FromCoords(enPassantCoordinates);
    }

    private int ParseClock(string clock) {
        if (int.TryParse(clock, out int res) && res >= 0) return res;
        throw new ArgumentException("A clock part of parsed FEN string is in incorrect format");
    }

    private void ParseCastles(string castlesString) {
        _state.WhiteCastleKingSide = castlesString.Contains('K');
        _state.WhiteCastleQueenSide = castlesString.Contains('Q');
        _state.BlackCastleKingSide = castlesString.Contains('k');
        _state.BlackCastleQueenSide = castlesString.Contains('q');
    }

    private void ParsePieces(string fenPieces) {
        var board = FenToMatrix(fenPieces);

        _state.WhiteQueens = ParsePiece(board, _state.WhiteQueens, State.WhiteQueenSymbol);
        _state.WhitePawns = ParsePiece(board, _state.WhitePawns, State.WhitePawnSymbol);
        _state.WhiteKing = ParsePiece(board, _state.WhiteKing, State.WhiteKingSymbol);
        _state.WhiteBishops = ParsePiece(board, _state.WhiteBishops, State.WhiteBishopSymbol);
        _state.WhiteKnights = ParsePiece(board, _state.WhiteKnights, State.WhiteKnightSymbol);
        _state.WhiteRooks = ParsePiece(board, _state.WhiteRooks, State.WhiteRookSymbol);
        _state.BlackQueens = ParsePiece(board, _state.BlackQueens, State.BlackQueenSymbol);
        _state.BlackPawns = ParsePiece(board, _state.BlackPawns, State.BlackPawnSymbol);
        _state.BlackKing = ParsePiece(board, _state.BlackKing, State.BlackKingSymbol);
        _state.BlackBishops = ParsePiece(board, _state.BlackBishops, State.BlackBishopSymbol);
        _state.BlackKnights = ParsePiece(board, _state.BlackKnights, State.BlackKnightSymbol);
        _state.BlackRooks = ParsePiece(board, _state.BlackRooks, State.BlackRookSymbol);
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