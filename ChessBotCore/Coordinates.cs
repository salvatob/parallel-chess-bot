using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore;

/// <summary>
/// Basic implementation of my board coordinates system.
/// a1 (bottom left) cell is at [0,0].
/// b1 (right of it) cell is at [0,1].
/// </summary>
public readonly struct Coordinates {
    public required int Row { get; init; }
    public required int Col { get; init; }

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


    public static Coordinates FromString(string square) {
        if (square.Length != 2) throw new Exception("square notation not parsed: str.length != 2");
        char c1 = square[0];
        char c2 = char.ToLower(square[1]);
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

    public int To1D() {
        return Row * 8 + Col;
    }

    public static int To1D(int i, int j) {
        return i * 8 + j;
    }

    public static Coordinates FromMask(Bitboard mask) {
        return From1D(mask.TrailingZeroCount());
    }
    
    public static Coordinates From1D(int coordinate) {
        return new() {
            Row = coordinate / 8,
            Col = 7 - coordinate % 8
        };
    }
}