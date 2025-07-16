using System.Diagnostics;
using System.Text;

namespace ChessBotCore;

public readonly record struct State {
    private const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
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

    
    public static State Initial() {
        return new State() {
            WhitePawns = 0b_1111_1111_0000_0000,
            WhiteRooks = 0b_1000_0001,
            WhiteKnights = 0b_0100_0010,
            WhiteBishops = 0b0010_0100,
            WhiteQueens = 0b0001_0000,
            WhiteKing = 0b_0000_1000,
            BlackPawns = (ulong)0b_1111_1111 << (5 * 8),
            BlackRooks = (ulong)0b_1000_0001  << (7 * 8),
            BlackKnights = (ulong)0b_0100_0010 << (7 * 8),
            BlackBishops = (ulong)0b0010_0100 << (7 * 8),
            BlackQueens = (ulong)0b0001_0000 << (7 * 8),
            BlackKing = (ulong)0b_0000_1000 << (7 * 8),
        };
    }

    public static State Empty() {
        return new State();
    }
    
    public State DeletePawns() {
        return this with { BlackPawns = 0, WhitePawns = 0 };
    }
}
