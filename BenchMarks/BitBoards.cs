using BenchmarkDotNet.Attributes;
using ChessBotCore;

namespace Benchmarks;

public class BitBoards {
    private static readonly Bitboard mask1 = Bitboard.FromCoords("h8");
    private static readonly Bitboard mask2 = Bitboard.FromCoords("a1");
    private static readonly Bitboard mask3 = Bitboard.FromCoords("d4");
    private static readonly Bitboard mask4 = Bitboard.FromCoords("e5");
    
    private static readonly State state1 = State.Initial;
    private static readonly State state2 = FenParser.ParseFen("8/8/4k3/8/2pPp3/8/B7/7K b - d3 0 1");
    private static readonly State state3 = FenParser.ParseFen("7k/8/8/8/pPp5/8/8/7K b - b3 0 1");


    
    private static readonly Bitboard[] Masks = [
        mask1,
        mask2,
        mask3,
        mask4,
    ];

    private static readonly State[] States = [
        state1,
        state2,
        state3,
    ];

    private static readonly (Bitboard Mask, State State)[] Cases = (
        from state in States
        from mask in Masks
        select (mask, state)
    ).ToArray();
    
    
    [Benchmark(Baseline = true)]
    public Pieces? RawBytes() {
        Pieces? result = null;

        foreach (var (mask, state) in Cases)
            result = RawBytesImpl(mask, state);

        return result;
    }

    [Benchmark]
    public Pieces? IsEmptyMethod() {
        Pieces? result = null;

        foreach (var (mask, state) in Cases)
            result = IsEmptyMethodImpl(mask, state);

        return result;
    }
    
    public Pieces? RawBytesImpl(Bitboard mask, State state) {
        if ((state.WhitePawns.RawBits & mask.RawBits) != 0) return Pieces.WhitePawns;
        if ((state.WhiteRooks.RawBits & mask.RawBits) != 0) return Pieces.WhiteRooks;
        if ((state.WhiteKnights.RawBits & mask.RawBits) != 0) return Pieces.WhiteKnights;
        if ((state.WhiteBishops.RawBits & mask.RawBits) != 0) return Pieces.WhiteBishops;
        if ((state.WhiteQueens.RawBits & mask.RawBits) != 0) return Pieces.WhiteQueens;
        if ((state.WhiteKing.RawBits & mask.RawBits) != 0) return Pieces.WhiteKing;
        if ((state.BlackPawns.RawBits & mask.RawBits) != 0) return Pieces.BlackPawns;
        if ((state.BlackRooks.RawBits & mask.RawBits) != 0) return Pieces.BlackRooks;
        if ((state.BlackKnights.RawBits & mask.RawBits) != 0) return Pieces.BlackKnights;
        if ((state.BlackBishops.RawBits & mask.RawBits) != 0) return Pieces.BlackBishops;
        if ((state.BlackQueens.RawBits & mask.RawBits) != 0) return Pieces.BlackQueens;
        if ((state.BlackKing.RawBits & mask.RawBits) != 0) return Pieces.BlackKing;
        return null;
    }

    public Pieces? IsEmptyMethodImpl(Bitboard mask, State state) {
        if (!(state.WhitePawns & mask).IsEmpty()) return Pieces.WhitePawns;
        if (!(state.WhiteRooks & mask).IsEmpty()) return Pieces.WhiteRooks;
        if (!(state.WhiteKnights & mask).IsEmpty()) return Pieces.WhiteKnights;
        if (!(state.WhiteBishops & mask).IsEmpty()) return Pieces.WhiteBishops;
        if (!(state.WhiteQueens & mask).IsEmpty()) return Pieces.WhiteQueens;
        if (!(state.WhiteKing & mask).IsEmpty()) return Pieces.WhiteKing;
        if (!(state.BlackPawns & mask).IsEmpty()) return Pieces.BlackPawns;
        if (!(state.BlackRooks & mask).IsEmpty()) return Pieces.BlackRooks;
        if (!(state.BlackKnights & mask).IsEmpty()) return Pieces.BlackKnights;
        if (!(state.BlackBishops & mask).IsEmpty()) return Pieces.BlackBishops;
        if (!(state.BlackQueens & mask).IsEmpty()) return Pieces.BlackQueens;
        if (!(state.BlackKing & mask).IsEmpty()) return Pieces.BlackKing;
        return null;
    }
}
