using System;

namespace ChessBotCore;

// Do not move the order of these flags, they define natural ordering so the sorting of Moves is very trivial 
[Flags]
public enum MoveFlags : ushort {
    Promotion = 1 << 15,
    Capture = 1 << 14,
    IsCheck = 1 << 13,
    DoublePawnPush = 1 << 12,
    KingCastle = 1 << 11,
    QueenCastle = 1 << 10,
    EnPassant = 1 << 9,
    PromoteToQueen = 1 << 8,
    PromoteToRook = 1 << 7,
    PromoteToBishop = 1 << 6,
    PromoteToKnight = 1 << 5,
    None = 0
}



public readonly struct Move : IComparable<Move> {
    // first 6 bits is from, next 6 is to, next 4 is Pieces, another 16 are flags
    private readonly uint _data;

    static bool GetColor(Pieces p) {
        return (byte)p > 6;
    }
    
    public Move(int from, int to, Pieces piece, MoveFlags flags = MoveFlags.None) {
        _data = (uint)((uint)(from | (to << 6) | ((byte)piece << 12)) | ((uint)flags << 16));
    }

    public int From => (int)(_data & 0x3F);
    public int To => (int)((_data >> 6) & 0x3F);
    public MoveFlags Flags => (MoveFlags)(_data >> 16);

    public int CompareTo(Move other) => other._data.CompareTo(_data); // Descending for high-priority moves first

    public bool IsCapture => Flags.HasFlag(MoveFlags.Capture);
    public bool IsPromotion => Flags.HasFlag(MoveFlags.Promotion);
    public bool IsCastle => Flags.HasFlag(MoveFlags.KingCastle) || Flags.HasFlag(MoveFlags.QueenCastle);
    public bool IsEnPassant => Flags.HasFlag(MoveFlags.EnPassant);
    public bool IsCheck => Flags.HasFlag(MoveFlags.IsCheck);
    public Pieces Piece => (Pieces)((_data >> 12) & 0b1111); // take only 4 bits ideally
    public bool IsWhite => GetColor(Piece);
    public static string? TryGetNotation(State before, State after) {
        return FenCreator.TryGetMoveNotation(before, after);
    }
}
