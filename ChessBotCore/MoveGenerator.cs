using ChessBotCore.Board;
using ChessBotCore.MoveGenerators;
using ChessBotCore.MoveGenerators.PieceGenerators;

namespace ChessBotCore;

/// <summary>
///     The default implementation of the <seealso cref="IMoveGenerator" /> interface,
///     providing access to both pseudo-legal and fully legal moves.
/// </summary>
public sealed class MoveGenerator : IMoveGenerator {
    /// <summary>
    ///     Default set of move generators for a standard chess game.
    /// </summary>
    private static readonly IReadOnlyList<IPieceMoveGenerator> DefaultGenerators = [
        KingMoveGenerator.Instance,
        KnightMoveGenerator.Instance,
        RookMoveGenerator.Instance,
        PawnMoveGenerator.Instance,
        QueenMoveGenerator.Instance,
        BishopMoveGenerator.Instance
    ];

    private readonly IPieceMoveGenerator[] _generators;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MoveGenerator" /> class for a given state using default generators.
    /// </summary>
    public MoveGenerator() : this(DefaultGenerators) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MoveGenerator" /> class with custom generators.
    /// </summary>
    /// <param name="generators">Custom move generators to use.</param>
    public MoveGenerator(IEnumerable<IPieceMoveGenerator> generators) {
        _generators = generators.ToArray();
    }

    /// <summary>
    ///     A shared, stateless default Singleton-like instance. 
    /// </summary>
    public static MoveGenerator Default { get; } = new();

    public MoveSet GenerateMoves(State state) {
        List<Move> buffer = new(40);
        foreach (var generator in _generators) generator.GenerateMoves(state, buffer);

        return new MoveSet(state, buffer);
    }

    /// <summary>
    ///     Checks if a pseudo-legal move is fully legal (i.e., it doesn't leave the king in check).
    /// </summary>
    /// <param name="move">The move to check.</param>
    /// <param name="state">The board state.</param>
    /// <returns>True if the move is legal, false otherwise.</returns>
    public static bool CheckMoveLegality(Move move, State state) {
        // castles are already checked
        if (move.IsCastle) return true;

        var undo = state.ApplyMove(move);

        // After ApplyMove, WhiteIsActive has flipped.
        // If white just moved, it's now black's turn. 
        // We need to check if white's king is under attack.
        var wasWhiteTurn = !state.WhiteIsActive;
        var kingBoard = wasWhiteTurn ? state.WhiteKing : state.BlackKing;

        bool legal;
        if (kingBoard.IsEmpty()) {
            legal = false; // Should not happen if king was there before
        }
        else {
            var kingSquare = kingBoard.TrailingZeroCount();
            legal = !IsSquareAttacked(kingSquare, state.WhiteIsActive, state);
        }

        state.UndoMove(move, undo);
        return legal;
    }

    /// <summary>
    ///     Checks if a specific square is under attack by a given player.
    /// </summary>
    /// <param name="square">The square index to check.</param>
    /// <param name="byWhite">True to check for white attackers, false for black.</param>
    /// <param name="state">The board state.</param>
    /// <returns>True if the square is attacked, false otherwise.</returns>
    internal static bool IsSquareAttacked(int square, bool byWhite, State state) {
        Bitboard squareMask = BitBoardHelpers.OneBitMask(square);
        Bitboard allPieces = state.GetAllPieces();

        // 1. Knights
        Bitboard knightAttackers = GetKnightAttacks(square) & (byWhite ? state.WhiteKnights : state.BlackKnights);
        if (!knightAttackers.IsEmpty()) return true;

        // 2. Pawns
        if (byWhite) {
            if (!((squareMask.MovePieces(Direction.SW) & state.WhitePawns).IsEmpty() &&
                  (squareMask.MovePieces(Direction.SE) & state.WhitePawns).IsEmpty()))
                return true;
        } else {
            if (!((squareMask.MovePieces(Direction.NW) & state.BlackPawns).IsEmpty() &&
                  (squareMask.MovePieces(Direction.NE) & state.BlackPawns).IsEmpty()))
                return true;
        }

        // 3. King
        Bitboard kingAttackers = GetKingAttacks(square) & (byWhite ? state.WhiteKing : state.BlackKing);
        if (!kingAttackers.IsEmpty()) return true;

        // 4. Sliding Pieces (Rooks, Bishops, Queens)
        // Orthogonal (Rook/Queen)
        Direction[] orthoDirs = [Direction.N, Direction.S, Direction.E, Direction.W];
        Bitboard orthoSliders = byWhite ? state.WhiteRooks | state.WhiteQueens : state.BlackRooks | state.BlackQueens;
        foreach (var dir in orthoDirs)
            if (!GetSliderAttack(square, dir, allPieces, orthoSliders).IsEmpty())
                return true;

        // Diagonal (Bishop/Queen)
        Direction[] diagDirs = [Direction.NE, Direction.NW, Direction.SE, Direction.SW];
        Bitboard diagSliders = byWhite ? state.WhiteBishops | state.WhiteQueens : state.BlackBishops | state.BlackQueens;
        foreach (var dir in diagDirs) {
            if (!GetSliderAttack(square, dir, allPieces, diagSliders).IsEmpty())
                return true;
        }

        return false;
    }

    /// <summary>
    ///     Gets a bitboard of squares attacked by a knight on a given square.
    /// </summary>
    private static Bitboard GetKnightAttacks(int square) {
        Bitboard mask = BitBoardHelpers.OneBitMask(square);
        return BitBoardHelpers.Move(mask, Direction.NNE) |
               BitBoardHelpers.Move(mask, Direction.NEE) |
               BitBoardHelpers.Move(mask, Direction.SEE) |
               BitBoardHelpers.Move(mask, Direction.SSE) |
               BitBoardHelpers.Move(mask, Direction.NNW) |
               BitBoardHelpers.Move(mask, Direction.NWW) |
               BitBoardHelpers.Move(mask, Direction.SWW) |
               BitBoardHelpers.Move(mask, Direction.SSW);
    }

    /// <summary>
    ///     Gets a bitboard of squares attacked by a king on a given square.
    /// </summary>
    private static Bitboard GetKingAttacks(int square) {
        Bitboard mask = BitBoardHelpers.OneBitMask(square);
        return BitBoardHelpers.Move(mask, Direction.N) |
               BitBoardHelpers.Move(mask, Direction.S) |
               BitBoardHelpers.Move(mask, Direction.E) |
               BitBoardHelpers.Move(mask, Direction.W) |
               BitBoardHelpers.Move(mask, Direction.NE) |
               BitBoardHelpers.Move(mask, Direction.NW) |
               BitBoardHelpers.Move(mask, Direction.SE) |
               BitBoardHelpers.Move(mask, Direction.SW);
    }

    /// <summary>
    ///     Checks for slider attacks along a ray in a specified direction.
    /// </summary>
    private static Bitboard GetSliderAttack(int square, Direction dir, Bitboard allPieces, Bitboard attackers) {
        Bitboard ray = BitBoardHelpers.OneBitMask(square);
        while (true) {
            ray = ray.MovePieces(dir);
            if (ray.IsEmpty()) break;
            if (!(ray & attackers).IsEmpty()) return ray;
            if (!(ray & allPieces).IsEmpty()) break; // Blocked by some piece
        }

        return Bitboard.Empty;
    }
}
