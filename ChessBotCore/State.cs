using System.Diagnostics;
using System.Numerics;

namespace ChessBotCore;

public readonly record struct State {
    public const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const char WhiteQueenSymbol = 'Q';
    public const char WhiteKingSymbol = 'K';
    public const char WhitePawnSymbol = 'P';
    public const char WhiteBishopSymbol = 'B';
    public const char WhiteKnightSymbol = 'N';
    public const char WhiteRookSymbol = 'R';

    public const char BlackQueenSymbol = 'q';
    public const char BlackKingSymbol = 'k';
    public const char BlackPawnSymbol = 'p';
    public const char BlackBishopSymbol = 'b';
    public const char BlackKnightSymbol = 'n';
    public const char BlackRookSymbol = 'r';

    [Obsolete($"{nameof(State)}.{nameof(Empty)} should be used instead of the constructor", false)]
    public State(int i) {
        WhitePawns = 0;
        WhiteRooks = 0;
        WhiteKnights = 0;
        WhiteBishops = 0;
        WhiteQueens = 0;
        WhiteKing = 0;
        BlackPawns = 0;
        BlackRooks = 0;
        BlackKnights = 0;
        BlackBishops = 0;
        BlackQueens = 0;
        BlackKing = 0;
        WhiteIsActive = false;
        WhiteCastleKingSide = false;
        WhiteCastleQueenSide = false;
        BlackCastleKingSide = false;
        BlackCastleQueenSide = false;
    }

    public ulong WhitePawns { get; init; }
    public ulong WhiteRooks { get; init; }
    public ulong WhiteKnights { get; init; }
    public ulong WhiteBishops { get; init; }
    public ulong WhiteQueens { get; init; }
    public ulong WhiteKing { get; init; }

    public ulong BlackPawns { get; init; }
    public ulong BlackRooks { get; init; }
    public ulong BlackKnights { get; init; }
    public ulong BlackBishops { get; init; }
    public ulong BlackQueens { get; init; }
    public ulong BlackKing { get; init; }

    public bool WhiteIsActive { get; init; }

    public bool WhiteCastleKingSide { get; init; }
    public bool WhiteCastleQueenSide { get; init; }
    public bool BlackCastleKingSide { get; init; }
    public bool BlackCastleQueenSide { get; init; }

    public ulong EnPassant { get; init; } = 0;

    public int HalfMovesSincePawnMoveOrCapture { get; init; } = 0;
    public int FullMoves { get; init; } = 1;

#pragma warning disable CS0612   // “obsolete” warning id
    public static State Initial =>
        new() {
            WhitePawns   = 0b_1111_1111_0000_0000,
            WhiteRooks   = 0b_1000_0001,
            WhiteKnights = 0b_0100_0010,
            WhiteBishops = 0b0010_0100,
            WhiteQueens  = 0b0001_0000,
            WhiteKing    = 0b_0000_1000,
            
            BlackPawns   = (ulong)0b_1111_1111 << (6 * 8),
            BlackRooks   = (ulong)0b_1000_0001 << (7 * 8),
            BlackKnights = (ulong)0b_0100_0010 << (7 * 8),
            BlackBishops = (ulong)0b_0010_0100 << (7 * 8),
            BlackQueens  = (ulong)0b_0001_0000 << (7 * 8),
            BlackKing    = (ulong)0b_0000_1000 << (7 * 8),
            
            WhiteCastleKingSide  = true,
            WhiteCastleQueenSide = true,
            BlackCastleKingSide  = true,
            BlackCastleQueenSide = true,
            WhiteIsActive = true,
            FullMoves = 1
        };
#pragma warning restore CS0612


#pragma warning disable CS0612   // “obsolete” warning id
    public static State Empty => new();

#pragma warning restore CS0612


    public bool EnPassantAvailable => EnPassant > 0;

    public Coordinates? GetEnPassantCoordinates() {
        return EnPassantAvailable
            ? MaskToCoordinates(EnPassant)
            : null;
    }

    /// <summary>
    ///     For a ulong 64-bit mask with only one bit true, this method returns its coordinates.
    /// </summary>
    /// <param name="mask">The mask, which only must have 1 bit true</param>
    /// <returns>The Coordinates of the masked bit</returns>
    public static Coordinates MaskToCoordinates(ulong mask) {
        Debug.Assert(HasSingleBit(mask));
        int coordinate1D = BitOperations.TrailingZeroCount(mask);
        return Coordinates.From1D(coordinate1D);
    }

    // from chatGPT
    private static bool HasSingleBit(ulong x) 
        => x != 0 && (x & (x - 1)) == 0;
}