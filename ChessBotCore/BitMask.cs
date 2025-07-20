using System.Numerics;

namespace ChessBotCore;

public static class BitMask {
    public static Bitboard[] Row { get; private set; }

    private static void InitRows() {
        Row = new Bitboard[8];
        for (int i = 0; i < 8; i++) 
            Row[i] = (ulong)(0b1111_1111UL << (8 * i));
    }
    
    
    public static Bitboard[] Col { get; private set; }

    private static void InitCols() {
        Col = new Bitboard[8];

        ulong col0 = 0UL;

        for (int i = 0; i < 8; i++) {
            col0 |= 1UL << (i * 8);
        }
        
        for (int i = 0; i < 8; i++) 
            Col[7-i] = BitOperations.RotateLeft(col0, i);
            // Col[i] = col0 >>> i;
    }
    
    static BitMask() {
        InitRows();
        InitCols();
    }
}

[Obsolete($"Would be an option, however the performance of {nameof(BitMask)} is better.", error:false)]
public static class RowMasksCompletelyStatic {
    public static readonly Bitboard ROW0 = 0xFFUL;
    public static readonly Bitboard ROW1 = 0xFF00UL;
    public static readonly Bitboard ROW2 = 0xFF0000UL;
    public static readonly Bitboard ROW3 = 0xFF000000UL;
    public static readonly Bitboard ROW4 = 0xFF00000000UL;
    public static readonly Bitboard ROW5 = 0xFF0000000000UL;
    public static readonly Bitboard ROW6 = 0xFF000000000000UL;
    public static readonly Bitboard ROW7 = 0xFF00000000000000UL;

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
