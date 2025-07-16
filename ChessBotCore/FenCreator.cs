using System.Diagnostics;
using System.Text;

namespace ChessBotCore;

public static class FenCreator {
    
    public static string GetFen(State state) {

        return "";
    }

    public static char[,] EncodeIntoMatrix(State state) {
        char[,] board = new char[8, 8];
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (IsPieceOnCoordinates(i, j, state.WhiteQueens)) {
                    board[i, j] = State.WhiteQueenSymbol;
                }
            }
        }

        return board;
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