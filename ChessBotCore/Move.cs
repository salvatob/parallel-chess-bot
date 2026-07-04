using System;

namespace ChessBotCore;

[Flags]
public enum MoveFlags : uint {
    None = 0,
    Capture = 1 << 0,
    DoublePawnPush = 1 << 1,
    KingCastle = 1 << 2,
    QueenCastle = 1 << 3,
    EnPassant = 1 << 4,
    Promotion = 1 << 5,
    PromoteToQueen = 1 << 6,
    PromoteToRook = 1 << 7,
    PromoteToBishop = 1 << 8,
    PromoteToKnight = 1 << 9,
    IsCheck = 1 << 10
}

public readonly struct Move {
    private readonly uint _data;

    public Move(int from, int to, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6) | ((uint)flags << 12));
    }

    public int From => (int)(_data & 0x3F);
    public int To => (int)((_data >> 6) & 0x3F);
    public MoveFlags Flags => (MoveFlags)(_data >> 12);

    public bool IsCapture => Flags.HasFlag(MoveFlags.Capture);
    public bool IsPromotion => Flags.HasFlag(MoveFlags.Promotion);
    public bool IsCastle => Flags.HasFlag(MoveFlags.KingCastle) || Flags.HasFlag(MoveFlags.QueenCastle);
    public bool IsEnPassant => Flags.HasFlag(MoveFlags.EnPassant);
    public bool IsCheck => Flags.HasFlag(MoveFlags.IsCheck);

    public static string? TryGetNotation(State before, State after) {
        return FenCreator.TryGetMoveNotation(before, after);
    }
}
