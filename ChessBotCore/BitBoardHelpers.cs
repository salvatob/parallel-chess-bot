using System.Diagnostics;
using System.Numerics;

namespace ChessBotCore;

public class BitBoardHelpers {
    public static ulong OneBitMask(int index) {
        return 1UL << index;
    }
    
    public static ulong OneBitMask(Coordinates coords) {
        return 1UL << coords.To1D();
    }

    /// <summary>
    /// Print ulong in a board representation. Used mainly for debugging.
    /// </summary>
    /// <param name="mask">The ulong to print</param>
    /// <param name="splitRows">If true, each row has a space in the middle</param>
    /// <returns>The string representation</returns>
    public static string PrintUlong(ulong mask, bool splitRows=false) {
        long bitsAsLong = (long)mask;
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
    public static ulong ParseUlong(string bitBoard) {
        ulong board = 0UL;
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
    ///     For a ulong 64-bit mask with only one bit true, this method returns its coordinates.
    /// </summary>
    /// <param name="mask">The mask, which only must have 1 bit true</param>
    /// <returns>The Coordinates of the masked bit</returns>
    public static Coordinates MaskToCoordinates(ulong mask) {
        Debug.Assert(HasSingleBit(mask));
        int coordinate1D = BitOperations.TrailingZeroCount(mask);
        return Coordinates.From1D(coordinate1D);
    }

    public static bool HasSingleBit(ulong x) 
        => x != 0 && (x & (x - 1)) == 0;
}

public static class ULongExtensions {
    public static string PrintAsBitBoard(this ulong val) {
        return BitBoardHelpers.PrintUlong(val);
    }
}