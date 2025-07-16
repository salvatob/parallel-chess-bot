using System.Diagnostics;
using System.Text;

namespace ChessBotCore;

public static class FenCreator {
    
    public static string GetFen(State state) {
        string pieces = EncodePieces(state);
        char activeColor = state.WhiteIsActive ? 'w' : 'b';
        string castles = EncodeCastles(state);
        string enpassant = EncodeEnPassant(state);
        string halfClock = state.HalfMovesSincePawnMoveOrCapture.ToString();
        string fullClock = state.FullMoves.ToString();
        
        return String.Join(" ", [pieces, activeColor, castles, enpassant, halfClock, fullClock]);
    }

    private static string EncodePieces(State state) {
        var matrix = EncodeIntoMatrix(state);
        var sb = EncodeMatrixIntoString(matrix);
        return sb.ToString();
    }

    private static string EncodeCastles(State state) {
        StringBuilder sb = new();
        if (state.WhiteCastleKingSide)   sb.Append('K');
        if (state.WhiteCastleQueenSide)   sb.Append('Q');
        if (state.BlackCastleKingSide)   sb.Append('k');
        if (state.BlackCastleQueenSide)   sb.Append('q');
        if (sb.Length == 0) return "-";
        return sb.ToString();
    }

    private static string EncodeEnPassant(State state) {
        Coordinates? enPassantCoordinates = state.GetEnPassantCoordinates();
        return enPassantCoordinates is null ? "-" : enPassantCoordinates.ToString()!;
    }
    
    private static void EncodeOtherStateAttributes(State state, StringBuilder sb) {
        
        sb.Append(' ');
        sb.Append(state.WhiteIsActive ? 'w' : 'b');
        sb.Append(' ');
        Coordinates? enPassantCoordinates = state.GetEnPassantCoordinates();
        sb.Append(
            enPassantCoordinates is null
                ? '-'
                : enPassantCoordinates.ToString());
        sb.Append(' ');
        
        if (state.WhiteCastleKingSide)   sb.Append('K');
        if (state.WhiteCastleQueenSide)   sb.Append('Q');
        if (state.BlackCastleKingSide)   sb.Append('k');
        if (state.BlackCastleQueenSide)   sb.Append('q');
    }
    
    public static char[,] EncodeIntoMatrix(State state) {
        char[,] board = new char[8, 8];
        
        EncodePiecesIntoMatrix(state.WhiteQueens, State.WhiteQueenSymbol, board);
        EncodePiecesIntoMatrix(state.WhitePawns, State.WhitePawnSymbol, board);
        EncodePiecesIntoMatrix(state.WhiteBishops, State.WhiteBishopSymbol, board);
        EncodePiecesIntoMatrix(state.WhiteKing, State.WhiteKingSymbol, board);
        EncodePiecesIntoMatrix(state.WhiteKnights, State.WhiteKnightSymbol, board);
        EncodePiecesIntoMatrix(state.WhiteRooks, State.WhiteRookSymbol, board);
        
        EncodePiecesIntoMatrix(state.BlackQueens, State.BlackQueenSymbol, board);
        EncodePiecesIntoMatrix(state.BlackPawns, State.BlackPawnSymbol, board);
        EncodePiecesIntoMatrix(state.BlackBishops, State.BlackBishopSymbol, board);
        EncodePiecesIntoMatrix(state.BlackKing, State.BlackKingSymbol, board);
        EncodePiecesIntoMatrix(state.BlackKnights, State.BlackKnightSymbol, board);
        EncodePiecesIntoMatrix(state.BlackRooks, State.BlackRookSymbol, board);
        
        return board;
    }


    private static void EncodePiecesIntoMatrix(ulong pieces, char pieceSymbol, char[,] board) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (IsPieceOnCoordinates(i, j, pieces)) {
                    if (board[i, j] != default)
                        throw new InvalidOperationException(
                            "A state has two pieces at the same place." +
                            $" {pieceSymbol} and{board[i, j]} are both on [{i},{j}]"
                        );
                    board[i, j] = pieceSymbol;
                }
            }
        }
        
    }
    public static StringBuilder EncodeMatrixIntoString(char[,] board) {
        Debug.Assert(board.GetLength(0) == 8 
                     && board.GetLength(1) == 8);

        StringBuilder sb = new(30);
        for (int i = 0; i < 8; i++) {
            int emptyCells = 0;
            for (int j = 0; j < 8; j++) {
                char cell = board[7-i, j];
                if (cell == default) { // cell is empty
                    if (j == 7) { // end of row
                        sb.Append(emptyCells + 1);
                        continue;
                    }
                    emptyCells++;
                    continue;
                }
                // means cell is occupied
                if (emptyCells > 0) sb.Append(emptyCells.ToString());
                sb.Append(cell);
                emptyCells = 0;
            }
            if (i<7) sb.Append('/');
        }

        return sb;
    }
    private static bool IsPieceOnCoordinates(int i, int j, ulong pieces) {
        ulong mask = (ulong)1 << (i * 8 + 7 - j);
        return (mask & pieces) > 0;
    }
}