using System.Numerics;

namespace ChessBotCore;

public static class BitMask {
    public static ulong[] Row { get; private set; }

    private static void InitRows() {
        Row = new ulong[8];
        for (int i = 0; i < 8; i++) 
            Row[i] = (ulong)(0b1111_1111UL << (8 * i));
    }
    
    
    public static ulong[] Col { get; private set; }

    private static void InitCols() {
        Col = new ulong[8];

        ulong col0 = 0UL;

        for (int i = 0; i < 8; i++) {
            col0 |= 1UL << (i * 8);
        }
        
        for (int i = 0; i < 8; i++) 
            Col[7-i] = BitOperations.RotateLeft(col0,i);
            // Col[i] = col0 >>> i;
    }
    
    
    
    static BitMask() {
        InitRows();
        InitCols();
    }

    
    /// <summary>
    /// Print ulong in a board representation. Used mainly for debugging.
    /// </summary>
    /// <param name="mask">The ulong to print</param>
    /// <returns>The string representation</returns>
    public static string PrintUlong(ulong mask) {
        long bitsAsLong = (long)mask;
        string whole = Convert.ToString(bitsAsLong,2).PadLeft(64, '0');
        string[] parts = new string[8];
        for (int i = 0; i < 8; i++) {
            parts[i] = whole[(i*8)..((i+1)*8)];
        }

        return string.Join(Environment.NewLine, parts);
    }
}

[Obsolete($"Would be an option, however the performance of {nameof(BitMask)} is better.", error:false)]
public static class RowMasksCompletelyStatic {
    public const ulong ROW0 = 0xFFUL;
    public const ulong ROW1 = 0xFF00UL;
    public const ulong ROW2 = 0xFF0000UL;
    public const ulong ROW3 = 0xFF000000UL;
    public const ulong ROW4 = 0xFF00000000UL;
    public const ulong ROW5 = 0xFF0000000000UL;
    public const ulong ROW6 = 0xFF000000000000UL;
    public const ulong ROW7 = 0xFF00000000000000UL;

    public static ulong GetMask(int row) => row switch {
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
