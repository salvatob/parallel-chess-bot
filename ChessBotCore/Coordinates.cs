using System.Diagnostics;

namespace ChessBotCore;

public readonly struct Coordinates {
    public required int Row { get; init; } 
    public required int Col { get; init; }
    
    public override string ToString() {
        Debug.Assert(Row is >= 0 and < 8, $"{nameof(Coordinates)}.{nameof(Row)} is outside bounds [0-7]");    
        Debug.Assert(Col is >= 0 and < 8, $"{nameof(Coordinates)}.{nameof(Col)} is outside bounds [0-7]");
        return Convert.ToChar(Col + 'a') + "" + (Row + 1);

    }
    
    
    public static Coordinates FromString(string square) {
        if (square.Length != 2) throw new Exception("square notation not parsed -- str.length != 2");
        return new Coordinates {
            Col = square[0] - 'a',
            Row = square[1] - '0' - 1
        };

    }
    
    
    public int To1D() {
        return Row * 8 + Col;
    }

    public static Coordinates From1D(int coordinate) {
        return new() {
            Row = coordinate / 8,
            Col = coordinate % 8
        };
    }
}