using System.Diagnostics;
using System.Numerics;

namespace ChessBotCore;

public enum Pieces {
    WhitePawns,
    WhiteRooks,
    WhiteKnights,
    WhiteBishops,
    WhiteQueens,
    WhiteKing,
    
    BlackPawns,
    BlackRooks,
    BlackKnights,
    BlackBishops,
    BlackQueens,
    BlackKing 
}

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
    public State(int _) {
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

    public Bitboard WhitePawns { get; init; }
    public Bitboard WhiteRooks { get; init; }
    public Bitboard WhiteKnights { get; init; }
    public Bitboard WhiteBishops { get; init; }
    public Bitboard WhiteQueens { get; init; }
    public Bitboard WhiteKing { get; init; }

    public Bitboard BlackPawns { get; init; }
    public Bitboard BlackRooks { get; init; }
    public Bitboard BlackKnights { get; init; }
    public Bitboard BlackBishops { get; init; }
    public Bitboard BlackQueens { get; init; }
    public Bitboard BlackKing { get; init; }

    public bool WhiteIsActive { get; init; }

    public bool WhiteCastleKingSide { get; init; }
    public bool WhiteCastleQueenSide { get; init; }
    public bool BlackCastleKingSide { get; init; }
    public bool BlackCastleQueenSide { get; init; }

    public Bitboard EnPassant { get; init; } = 0;

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
            
            BlackPawns   = 0b_1111_1111UL << (6 * 8),
            BlackRooks   = 0b_1000_0001UL << (7 * 8),
            BlackKnights = 0b_0100_0010UL << (7 * 8),
            BlackBishops = 0b_0010_0100UL << (7 * 8),
            BlackQueens  = 0b_0001_0000UL << (7 * 8),
            BlackKing    = 0b_0000_1000UL << (7 * 8),
            
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

    /// <summary>
    /// Returns a copy of current state, where some properties are updated automatically.
    /// <see cref="WhiteIsActive"/>, <see cref="FullMoves"/> clock, are updated automatically.
    /// Pieces are left untouched, user must change them on their own.
    /// Specific properties such as <seealso cref="EnPassant"/> or castling are also left to the user to handle.
    /// </summary>
    /// <returns>A copy with fullmove clock and active color automatically updated.</returns>
    public State Next() {
        return this with {
            WhiteIsActive = !this.WhiteIsActive,
            FullMoves = this.WhiteIsActive ? this.FullMoves : this.FullMoves + 1
        };
    }
    public bool EnPassantAvailable => EnPassant.RawBits != 0;

    public Coordinates? GetEnPassantCoordinates() {
        return EnPassantAvailable
            ? BitBoardHelpers.MaskToCoordinates(EnPassant)
            : null;
    }

    /// <summary>
    /// Return Bitboard of all pieces of certain color.
    /// </summary>
    /// <param name="white">If true, returns white pieces, else black pieces.</param>
    /// <returns>The combination bitmap of the pieces.</returns>
    public Bitboard GetPieces(bool white) {
        return white switch {
            true =>
                WhiteBishops |
                WhiteKnights |
                WhiteQueens |
                WhiteKing |
                WhitePawns |
                WhiteRooks,
            false => 
                BlackBishops |
                BlackKnights |
                BlackQueens |
                BlackKing |
                BlackPawns |
                BlackRooks,
        };
    }
    
    public Bitboard GetAllPieces() {
        return GetPieces(true) | GetPieces(false);
    }

    public Bitboard GetActivePieces() => GetPieces(WhiteIsActive);
    
    public Bitboard GetInactivePieces() => GetPieces(!WhiteIsActive);

    /// <summary>
    /// Determines whether any piece of current state collide with
    /// </summary>
    /// <param name="mask">The board to check against. Should be unary bitboard (only 1 bit set).</param>
    /// <returns>Pieces enum entry, if collision has been found, null if no pieces collide</returns>
    public Pieces? DetectPieces(Bitboard mask) {
        if ((WhitePawns & mask).Empty) return Pieces.WhitePawns;
        if ((WhiteRooks & mask).Empty) return Pieces.WhiteRooks;
        if ((WhiteKnights & mask).Empty) return Pieces.WhiteKnights;
        if ((WhiteBishops & mask).Empty) return Pieces.WhiteBishops;
        if ((WhiteQueens & mask).Empty) return Pieces.WhiteQueens;
        if ((WhiteKing & mask).Empty) return Pieces.WhiteKing;
        if ((BlackPawns & mask).Empty) return Pieces.BlackPawns;
        if ((BlackRooks & mask).Empty) return Pieces.BlackRooks;
        if ((BlackKnights & mask).Empty) return Pieces.BlackKnights;
        if ((BlackBishops & mask).Empty) return Pieces.BlackBishops;
        if ((BlackQueens & mask).Empty) return Pieces.BlackQueens;
        if ((BlackKing & mask).Empty) return Pieces.BlackKing;
        return null;
    }
    
    
    public State With(Pieces changedPieces, Bitboard newPieces) {
        return changedPieces switch {
            Pieces.WhitePawns => this with { WhitePawns = newPieces },
            Pieces.WhiteRooks => this with { WhiteRooks = newPieces },
            Pieces.WhiteKnights => this with { WhiteKnights = newPieces },
            Pieces.WhiteBishops => this with { WhiteBishops = newPieces },
            Pieces.WhiteQueens => this with { WhiteQueens = newPieces },
            Pieces.WhiteKing => this with { WhiteKing = newPieces },
            Pieces.BlackPawns => this with { BlackPawns = newPieces },
            Pieces.BlackRooks => this with { BlackRooks = newPieces },
            Pieces.BlackKnights => this with { BlackKnights = newPieces },
            Pieces.BlackBishops => this with { BlackBishops = newPieces },
            Pieces.BlackQueens => this with { BlackQueens = newPieces },
            Pieces.BlackKing => this with { BlackKing = newPieces },
            _ => throw new ArgumentOutOfRangeException(nameof(changedPieces), changedPieces, null)
        };
    }
    
    
    public Bitboard GetPieces(Pieces pieces) {
        return pieces switch {
            Pieces.WhitePawns => this.WhitePawns,
            Pieces.WhiteRooks => this.WhiteRooks,
            Pieces.WhiteKnights => this.WhiteKnights,
            Pieces.WhiteBishops => this.WhiteBishops,
            Pieces.WhiteQueens => this.WhiteQueens,
            Pieces.WhiteKing => this.WhiteKing,
            Pieces.BlackPawns => this.BlackPawns,
            Pieces.BlackRooks => this.BlackRooks,
            Pieces.BlackKnights => this.BlackKnights,
            Pieces.BlackBishops => this.BlackBishops,
            Pieces.BlackQueens => this.BlackQueens,
            Pieces.BlackKing => this.BlackKing,
            _ => throw new ArgumentOutOfRangeException(nameof(pieces), pieces, null)
        };
    }

}