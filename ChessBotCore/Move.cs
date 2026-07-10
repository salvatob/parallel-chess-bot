using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

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


    public Move(int from, int to, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6)) | ((uint)flags << 16);
    }

    public Move(int from, int to, Pieces piece, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6) | ((byte)piece << 12)) | ((uint)flags << 16);
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

    private string GetPromotionNotation() => IsPromotion ? (
    Flags.HasFlag(MoveFlags.PromoteToQueen) ? "q" : Flags.HasFlag(MoveFlags.PromoteToRook) ? "r" : Flags.HasFlag(MoveFlags.PromoteToBishop) ? "b" : "n"
    ) : "";
    public override string ToString() => $"{Piece}-{PrintUCI()} {Flags}";
    // ReSharper disable once InconsistentNaming
    public string PrintUCI() => $"{Coordinates.From1D(From)}{Coordinates.From1D(To)}{GetPromotionNotation()}";

    public static Move Parse(string move) {
        string moveRegex = "([a-h][1-8])([a-h][1-8])([qrbn]?)";
        var match = Regex.Match(move, moveRegex);
        
        if (move.Length is not (4 or 5) || !match.Success) 
            throw new ArgumentException($"Invalid move notation: <{move}>");
        
        var fromS = match.Groups[1].Value;
        var toS = match.Groups[2].Value;
        var promotionS = match.Groups[3].Value;

        var from = Coordinates.FromString(fromS).To1D();
        var to = Coordinates.FromString(toS).To1D();
        var promotionFlag = promotionS switch {
            "" => MoveFlags.None,
            "q" => MoveFlags.PromoteToQueen,
            "r" => MoveFlags.PromoteToRook,
            "n" => MoveFlags.PromoteToKnight,
            "b" => MoveFlags.PromoteToBishop,
            _ => throw new UnreachableException($"Invalid promotion notation: <{promotionS}>")
        };

        return new Move(from, to, promotionFlag);
    }
}
