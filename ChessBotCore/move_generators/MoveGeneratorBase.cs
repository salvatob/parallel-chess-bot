namespace ChessBotCore;

/// <summary>
/// A base of all main move generators. All non-abstract inheritors are expected be sealed and
/// to implement the Singleton pattern.
/// </summary>
public abstract class MoveGeneratorBase : IMoveGenerator {
    protected abstract Pieces WhitePiece { get; }
    protected abstract Pieces BlackPiece { get; }
    
    public abstract IEnumerable<Move> GenerateMoves(State state);

    
    /// <summary>
    /// Delegades a creation of a Move out of a bitboard masks.
    /// </summary>
    /// <param name="maskBefore">The mask of the piece before moving</param>
    /// <param name="maskAfter">The mask of the piece aftermoving</param>
    /// <param name="state">The state before the move</param>
    /// <returns>A <see cref="Move"/> struct</returns>
    protected Move CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
        bool white = state.WhiteIsActive;
        Pieces? capture = state.DetectPieceCollision(maskAfter);

        // TODO when State becomes a managed class, this needs so be explicit copy
        State nextState = state.Clone();
        Bitboard boardAfterTheMove;
        // adds the moved piece, removes the before piece
        if (white) {
            boardAfterTheMove = (state.GetPieces(WhitePiece) | maskAfter) & (~maskBefore);

            nextState.Next();
            nextState.Set(WhitePiece, boardAfterTheMove);
        } else {
            boardAfterTheMove = (state.GetPieces(BlackPiece) | maskAfter) & (~maskBefore);

            nextState.Next();
            nextState.Set(BlackPiece, boardAfterTheMove);
        }
        // additionally removes the captured piece from its corresponding bitboard 
        bool isCapture = capture.HasValue;
        if (isCapture) {
            var newCapturedPieces = state.GetPieces(capture!.Value) & ~maskAfter;
            nextState.Set(capture.Value, newCapturedPieces);
        }

        if (isCapture) nextState.HalfClockReset();
        
        return new Move(nextState) {
            IsCapture = isCapture,
            coordsBefore = Coordinates.FromMask(maskBefore),
            coordsAfter = Coordinates.FromMask(maskAfter)
        };
    }

}