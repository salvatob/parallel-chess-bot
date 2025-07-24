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
        Bitboard withoutLeftCol = ~ BitMask.Col[0];
        Bitboard withoutRightCol = ~ BitMask.Col[7];
        Bitboard withoutTwoLeftCols = ~ (BitMask.Col[0] | BitMask.Col[1]);
        Bitboard withoutTwoRightCols = ~ (BitMask.Col[6] | BitMask.Col[7]);;
            
        return dir switch {
            Direction.N => bits << 8,
            Direction.S => bits >> 8,
            Direction.E => (bits >> 1) & withoutLeftCol,
            Direction.W => (bits << 1) & withoutRightCol,
            
            Direction.NE => (bits << 7) & withoutLeftCol,
            Direction.NW => (bits << 9) & withoutRightCol,
            Direction.SE => (bits >> 9) & withoutLeftCol,
            Direction.SW => (bits >> 7) & withoutRightCol,
            
            Direction.NNE =>  (bits << 17) & withoutLeftCol ,
            Direction.NEE =>  (bits << 10) & withoutTwoRightCols,
            Direction.SEE =>  (bits >>  6) & withoutTwoRightCols,
            Direction.SSE =>  (bits >> 15) & withoutLeftCol ,
            Direction.NNW =>  (bits << 15) & withoutRightCol ,
            Direction.NWW =>  (bits <<  6) & withoutTwoLeftCols,
            Direction.SWW =>  (bits >> 10) & withoutTwoLeftCols,
            Direction.SSW =>  (bits >> 17) & withoutRightCol ,
            
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
    
    public static Direction OppositeDir(Direction dir) {
        return dir switch {
            Direction.N => Direction.S,
            Direction.S => Direction.N,
            Direction.W => Direction.E,
            Direction.E => Direction.W,
            Direction.NE => Direction.SW,
            Direction.NW => Direction.SE,
            Direction.SE => Direction.NW,
            Direction.SW => Direction.NE,
            
            Direction.SSW => Direction.NNE,
            Direction.SWW => Direction.NEE,
            Direction.NWW => Direction.SEE,
            Direction.NNW => Direction.SSE,
            Direction.SSE => Direction.NNW,
            Direction.SEE => Direction.NWW,
            Direction.NEE => Direction.SWW,
            Direction.NNE => Direction.SSW,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
}



public enum Direction {
    N, S, W, E,
    NE, NW, SE, SW,
    NNE, NEE, NNW, NWW,
    SSE, SEE, SSW, SWW
};

