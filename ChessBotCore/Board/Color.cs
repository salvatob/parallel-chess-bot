using System.Runtime.CompilerServices;

namespace ChessBotCore.Board;


public enum Color : byte {
    White,
    Black
}

public static class ColorEnumExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Opposite(this Color color) {
        return color == Color.White ? Color.Black : Color.White;
    }
}
