using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessBotCore.Board;

/// <summary>
/// Represents a 64-bit chessboard using a bitboard representation.
/// </summary>
[DebuggerDisplay("{DebugPrint(),nq}")]
public readonly struct Bitboard : IEquatable<Bitboard>, IBitwiseOperators<Bitboard, Bitboard, Bitboard> {
    private readonly ulong _bits;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bitboard"/> struct with the specified bits.
    /// </summary>
    /// <param name="bits">The 64-bit value representing the board.</param>
    public Bitboard(ulong bits) => _bits = bits;

    /// <summary>Expose the raw bits when you really need them.</summary>
    public ulong RawBits {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bits;
    }

    /// <summary>
    /// Checks if the bitboard has no bits set.
    /// </summary>
    /// <returns>True if the bitboard is empty, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty() => _bits == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bitboard(ulong bits) 
        => new Bitboard(bits);

    /// <summary>
    /// Creates a bitboard from a coordinate string (e.g., "e4").
    /// </summary>
    /// <param name="coords">The coordinate string.</param>
    /// <returns>A bitboard with the bit corresponding to the coordinate set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard FromCoords(string coords) => FromCoords(Coordinates.FromString(coords));
    
    /// <summary>
    /// Creates a bitboard from a <see cref="Coordinates"/> object.
    /// </summary>
    /// <param name="coords">The coordinates.</param>
    /// <returns>A bitboard with the bit corresponding to the coordinates set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard FromCoords(Coordinates coords) {
        ulong bits = 1UL << (7 - coords.Col + 8 * coords.Row);
        return new Bitboard(bits);
    }

    /// <summary>
    /// Gets an empty bitboard (all bits zero).
    /// </summary>
    public static Bitboard Empty => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator |(Bitboard a, Bitboard b)
        => new Bitboard(a._bits | b._bits);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator ^(Bitboard a, Bitboard b)
        => new(a._bits ^ b._bits);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator &(Bitboard a, Bitboard b)
        => new(a._bits & b._bits);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator ~(Bitboard b)
        => new(~b._bits);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator <<(Bitboard b, int dist)
        => new Bitboard(b._bits << dist);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard operator >>(Bitboard b, int dist)
        => new Bitboard(b._bits >> dist);
    
    /// <summary>
    /// Returns the number of bits set to 1 in the bitboard.
    /// </summary>
    /// <returns>The population count.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PopCount()
        => BitOperations.PopCount(_bits);
    
    /// <summary>
    /// Returns the number of trailing zero bits in the bitboard, which corresponds to the index of the first set bit.
    /// </summary>
    /// <returns>The trailing zero count.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int TrailingZeroCount()
        => BitOperations.TrailingZeroCount(_bits);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bitboard other) 
        => _bits == other._bits;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) 
        => obj is Bitboard bb && Equals(bb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() 
        => _bits.GetHashCode();
    
    /// <summary>
    /// Returns a string representation of the bitboard as a grid for visualization.
    /// </summary>
    /// <returns>A grid string.</returns>
    public string PrettyPrint() {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("  a b c d e f g h");
        for (int rank = 7; rank >= 0; rank--) {
            sb.Append(rank + 1).Append(' ');
            for (int file = 7; file >= 0; file--) {
                int sq = rank * 8 + file;
                bool set = ((RawBits >> sq) & 1UL) != 0;
                sb.Append(set ? '■' : '-').Append(' ');
            }
            
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public string Print(bool splitRows = false) {
        var bitsAsLong = (long)RawBits;
        var whole = Convert.ToString(bitsAsLong, 2).PadLeft(64, '0');
        var parts = new string[8];
        for (var i = 0; i < 8; i++) {
            var row = whole[(i * 8)..((i + 1) * 8)];
            if (splitRows)
                row = row.Insert(4, " ");
            parts[i] = row;
        }

        return string.Join(Environment.NewLine, parts);
    }


    /// <summary>
    /// Shifts all bits in the bitboard in the specified direction.
    /// </summary>
    /// <param name="dir">The direction to move the pieces.</param>
    /// <returns>A new bitboard with the shifted bits.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bitboard MovePieces(Direction dir) => BitBoardHelpers.Move(this, dir);
    
    /// <summary>
    /// Parses a string representation of a bitboard.
    /// </summary>
    /// <param name="bitboard">The string to parse.</param>
    /// <returns>The parsed bitboard.</returns>
    public static Bitboard Parse(string bitboard) => BitBoardHelpers.ParseBoard(bitboard);

    /// <summary>
    /// Returns a short string description of the bitboard for debugging purposes.
    /// </summary>
    /// <returns>A debug string.</returns>
    public string DebugPrint() {
        if (_bits == 0) return "Empty board.";
        if (PopCount() == 1) {
            var coords = Coordinates.From1D(TrailingZeroCount());
            return $"one bit on {coords.ToString()}.";
        }

        if (PopCount() == 63) {
            var negative = ~this;
            var coords = Coordinates.From1D(negative.TrailingZeroCount());
            return $"Negative mask of {coords.ToString()}.";
        }
        
        return "0x" + _bits.ToString("X");
    }

    public override string ToString() {
        return PrettyPrint();
    }
}
