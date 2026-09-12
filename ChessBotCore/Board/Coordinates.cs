using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Board;

/// <summary>
///     Basic implementation of my board coordinates system.
///     a1 (bottom left) cell is at [0,0].
///     b1 (right of it) cell is at [0,1].
/// </summary>
public readonly struct Coordinates {
    /// <summary> The row index (0-7, where 0 is rank 1). </summary>
    public required int Row { get; init; }
    /// <summary> The column index (0-7, where 0 is file a). </summary>
    public required int Col { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> struct.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    [SetsRequiredMembers]
    public Coordinates(int row, int col) {
        Row = row;
        Col = col;
    }

    public override string ToString() {
        Debug.Assert(Row is >= 0 and < 8, $"{nameof(Coordinates)}.{nameof(Row)} is outside bounds [0-7]");
        Debug.Assert(Col is >= 0 and < 8, $"{nameof(Coordinates)}.{nameof(Col)} is outside bounds [0-7]");
        return Convert.ToChar(Col + 'a') + "" + (Row + 1);
    }


    /// <summary>
    /// Parses a square notation string (e.g., "e4") into coordinates.
    /// </summary>
    /// <param name="square">The square notation string.</param>
    /// <returns>The coordinates corresponding to the string.</returns>
    /// <exception cref="ArgumentException">Thrown if the square notation is invalid.</exception>
    public static Coordinates FromString(string square) {
        if (square.Length != 2) throw new ArgumentException("square notation not parsed: str.length != 2");
        var c1 = char.ToLower(square[0]);
        char c2 = square[1];
        if (!char.IsLetter(c1) || c2 > 'h')
            throw new ArgumentException("square notation not parsed: " +
                                        "first character must be a letter between a-h");
        if (!char.IsDigit(c2))
            throw new ArgumentException("square notation not parsed: " +
                                        "second character must be a digit");
        return new Coordinates {
            Col = c1 - 'a',
            Row = c2 - '0' - 1
        };
    }

    /// <summary>
    /// Converts the 2D coordinates to a 1D index (0-63).
    /// </summary>
    /// <returns>The 1D index.</returns>
    public int To1D() {
        return Row * 8 + (7 - Col);
    }

    /// <summary>
    /// Static helper to convert row and column indices to a 1D index.
    /// </summary>
    /// <param name="i">The row index.</param>
    /// <param name="j">The column index.</param>
    /// <returns>The 1D index.</returns>
    public static int To1D(int i, int j) {
        return i * 8 + j;
    }

    /// <summary>
    /// Creates coordinates from a bitboard mask containing a single bit.
    /// </summary>
    /// <param name="mask">The bitboard mask.</param>
    /// <returns>The coordinates of the set bit.</returns>
    public static Coordinates FromMask(Bitboard mask) {
        return From1D(mask.TrailingZeroCount());
    }

    /// <summary>
    /// Creates coordinates from a 1D index (0-63).
    /// </summary>
    /// <param name="coordinate">The 1D index.</param>
    /// <returns>The coordinates.</returns>
    public static Coordinates From1D(int coordinate) {
        return new Coordinates {
            Row = coordinate / 8,
            Col = 7 - coordinate % 8
        };
    }
}
