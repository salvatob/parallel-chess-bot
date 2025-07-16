using System.Diagnostics;
using System.Text;

namespace ChessBotCore;

public static class FenCreator {
    
    public static string GetFen(State state) {

        throw new NotImplementedException();
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
    public static StringBuilder EncodePieces(char[,] board) {
        Debug.Assert(board.GetLength(0) == 8 
                     && board.GetLength(1) == 8);

        StringBuilder sb = new(30);
        for (int i = 0; i < 8; i++) {
            int emptyCells = 0;
            for (int j = 0; j < 8; j++) {
                char cell = board[i, j];
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