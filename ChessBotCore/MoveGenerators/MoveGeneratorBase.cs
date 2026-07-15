using ChessBotCore.Board;

namespace ChessBotCore.MoveGenerators;

/// <summary>
/// A base of all main move generators. All non-abstract inheritors are expected be sealed and
/// to implement the Singleton pattern.
/// </summary>
public abstract class MoveGeneratorBase : IMoveGenerator {
    protected abstract Pieces WhitePiece { get; }
    protected abstract Pieces BlackPiece { get; }
    
    public abstract void GenerateMoves(State state, List<Move> buffer);

    
    /// <summary>
    /// Delegades a creation of a Move out of a bitboard masks.
    /// </summary>
    /// <param name="maskBefore">The mask of the piece before moving</param>
    /// <param name="maskAfter">The mask of the piece aftermoving</param>
    /// <param name="state">The state before the move</param>
    /// <returns>A <see cref="Move"/> struct</returns>
    protected Move CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
        Pieces pieceType = state.WhiteIsActive ? WhitePiece : BlackPiece; 
        Pieces? capture = state.DetectPieceCollision(maskAfter);
        MoveFlags flags = MoveFlags.None;
        if (capture.HasValue) flags |= MoveFlags.Capture;

        return new Move(maskBefore.TrailingZeroCount(), maskAfter.TrailingZeroCount(), pieceType, flags);
    }

}