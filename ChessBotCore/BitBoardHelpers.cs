using System.Diagnostics;

namespace ChessBotCore;

/// <summary>
/// Includes several static methods for the <see cref="Bitboard"/> struct.
/// All methods outside ChessBotCore assembly should be exposed through the that structure.
/// </summary>
internal static class BitBoardHelpers {
    public static Bitboard OneBitMask(int index) {
        return 1UL << index;
    }
    
    public static Bitboard OneBitMask(Coordinates coords) {
        return 1UL << coords.To1D();
    }

    /// <summary>
    /// Print Bitboard in a board representation. Used mainly for debugging.
    /// </summary>
    /// <param name="mask">The Bitboard to print</param>
    /// <param name="splitRows">If true, each row has a space in the middle</param>
    /// <returns>The string representation</returns>
    public static string PrintUlong(Bitboard mask, bool splitRows=false) {
        long bitsAsLong = (long)mask.RawBits;
        string whole = Convert.ToString(bitsAsLong,2).PadLeft(64, '0');
        string[] parts = new string[8];
        for (int i = 0; i < 8; i++) {
            string row = whole[(i*8)..((i+1)*8)];
            if (splitRows) 
                row = row.Insert(4, " ");
            parts[i] = row;
        }

        return string.Join(Environment.NewLine, parts);
    }

    /// <summary>
    /// Parses a string representation of a bitboard. All characters apart from {'0', '1'} are ignored.
    /// Must contain exactly 64 of these binary digits.
    /// </summary>
    /// <param name="bitBoard">The string of the bitboard</param>
    /// <returns>A Ulong representation of the parsed bitboard.</returns>
    /// <exception cref="ArgumentException">The string does not contain exactly 64 binary digits.</exception>
    internal static Bitboard ParseBoard(string bitBoard) {
        Bitboard board = 0UL;
        var chars = bitBoard.Where(c => c is '0' or '1').ToArray();
        if (chars.Length != 64)
            throw new ArgumentException($"The {nameof(bitBoard)} string needs to contain exactly 64 binary digits");
        for (int i = 0; i < 64; i++) {
            if (chars[63-i] == '1')
                board |= OneBitMask(i);
        }

        return board;
    }
    
    
    /// <summary>
    ///     For a Bitboard 64-bit mask with only one bit true, this method returns its coordinates.
    /// </summary>
    /// <param name="mask">The mask, which only must have 1 bit true</param>
    /// <returns>The Coordinates of the masked bit</returns>
    public static Coordinates MaskToCoordinates(Bitboard mask) {
        Debug.Assert(HasSingleBit(mask));
        int coordinate1D = mask.TrailingZeroCount();
        return Coordinates.From1D(coordinate1D);
    }

    /// <summary>
    /// Determines, if the Bitboard has exactly one bit set to true.
    /// </summary>
    /// <param name="x">The Bitboard to determine</param>
    /// <returns>True, if it has one bit</returns>
    public static bool HasSingleBit(Bitboard x) 
        => x.RawBits != 0 && (x.RawBits & (x.RawBits - 1)) == 0;

    public static Bitboard Move(Bitboard bits, Direction dir) {
        return dir switch {
            Direction.N => bits << 8,
            Direction.S => bits >> 8,
            Direction.E => (bits >> 1) & (~ BitMask.Col[7]) ,
            Direction.W => (bits << 1) & (~ BitMask.Col[0]),
            Direction.NE => (bits << 7) & (~ BitMask.Col[7]),
            // Direction.NW => expr,
            // Direction.SE => expr,
            // Direction.SW => expr,
            // Direction.NNE => expr,
            // Direction.NEE => expr,
            // Direction.NNW => expr,
            // Direction.NWW => expr,
            // Direction.SSE => expr,
            // Direction.SEE => expr,
            // Direction.SSW => expr,
            // Direction.SWW => expr,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
}

public enum Direction {
    N,S,W,E,
    NE,NW,SE,SW,
    NNE,NEE,NNW,NWW,
    SSE,SEE,SSW,SWW
};

