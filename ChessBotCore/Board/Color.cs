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
    
    /// <summary>
    /// Selects the parameter corresponding to the current color.
    /// </summary>
    /// <param name="color">The current color parameter</param>
    /// <param name="white">Value returned if Color is White</param>
    /// <param name="black">Value returned if Color is White</param>
    /// <typeparam name="T">Type of the values to choose from.</typeparam>
    /// <returns>the parameter determined by the current color</returns>
    public static T Choose<T>(this Color color, T white, T black) =>
        color == Color.White ? white : black;
    
    
    public static ref T Choose<T>(this Color color, ref T white, ref T black)
        => ref color.IsWhite() ? ref white : ref black;
}
