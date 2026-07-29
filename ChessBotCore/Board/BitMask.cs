using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ChessBotCore.Board;

public static class BitMask {
    static BitMask() {
        Row = InitRows();
        Col = InitCols();
    }

    public static Bitboard[] Row { get; private set; }


    public static Bitboard[] Col { get; private set; }

    private static Bitboard[] InitRows() {
        var row = new Bitboard[8];
        for (int i = 0; i < 8; i++)
            row[i] = 0b1111_1111UL << (8 * i);
        return row;
    }

    private static Bitboard[] InitCols() {
        var col = new Bitboard[8];

        ulong col0 = 0UL;

        for (int i = 0; i < 8; i++) {
            col0 |= 1UL << (i * 8);
        }

        for (int i = 0; i < 8; i++) { 
            col[7 - i] = BitOperations.RotateLeft(col0, i);
        }
        
        return col;
    }
}

[Obsolete($"Would be an option, however the performance of {nameof(BitMask)} is better.", error:false)]
public static class RowMasksCompletelyStatic {
    // ReSharper disable InconsistentNaming
    private static readonly Bitboard ROW0 = 0xFFUL;
    private static readonly Bitboard ROW1 = 0xFF00UL;
    private static readonly Bitboard ROW2 = 0xFF0000UL;
    private static readonly Bitboard ROW3 = 0xFF000000UL;
    private static readonly Bitboard ROW4 = 0xFF00000000UL;
    private static readonly Bitboard ROW5 = 0xFF0000000000UL;
    private static readonly Bitboard ROW6 = 0xFF000000000000UL;
    private static readonly Bitboard ROW7 = 0xFF00000000000000UL;
    // ReSharper restore InconsistentNaming

    public static Bitboard GetMask(int row) => row switch {
        0 => ROW0,
        1 => ROW1,
        2 => ROW2,
        3 => ROW3,
        4 => ROW4,
        5 => ROW5,
        6 => ROW6,
        7 => ROW7,
        _ => throw new ArgumentOutOfRangeException(nameof(row))
    };
}
