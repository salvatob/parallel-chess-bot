using System.Runtime.CompilerServices;

namespace ChessBotCore.Board;

/// <summary>
/// A boolean-like enum representing a chess player color/side
/// </summary>
public enum Color : byte {
    // having this enum, opposed to a simple bool, offers identical performance, however offers significantly more ergonomic
    // interface, by using extension methods, and also better readability
    // for historical reasons, where the boolean true used to represent the white player, Black is first in this enum
    Black,
    White
}

public static class ColorEnumExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Opposite(this Color color) {
        return color == Color.White ? Color.Black : Color.White;
    }
    public static bool IsWhite(this Color color) => color == Color.White;
    public static bool IsBlack(this Color color) => color == Color.Black;
}
