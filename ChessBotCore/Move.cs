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
    private readonly uint _data;

    public Move(int from, int to, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6) | ((uint)flags << 16));
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

    public static string? TryGetNotation(State before, State after) {
        return FenCreator.TryGetMoveNotation(before, after);
    }
}
