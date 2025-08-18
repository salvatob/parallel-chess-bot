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
        var capture = state.DetectPieces(maskAfter);

        State nextState;
        Bitboard boardAfterTheMove;
        // adds the moved piece, removes the before piece
        if (white) {
            boardAfterTheMove = (state.GetPieces(WhitePiece) | maskAfter) & (~maskBefore);

            nextState = state.Next().With(WhitePiece, boardAfterTheMove);
        } else {
            boardAfterTheMove = (state.GetPieces(BlackPiece) | maskAfter) & (~maskBefore);

            nextState = state.Next().With(BlackPiece, boardAfterTheMove);
        }
        // additionally removes the captured piece from its corresponding bitboard 
        bool isCapture = capture.HasValue;
        if (isCapture) {
            var newCapturedPieces = state.GetPieces(capture!.Value) & ~maskAfter;
            nextState = nextState.With(capture.Value, newCapturedPieces);
        }

        if (isCapture) nextState = nextState.WithHalfClockReset();
        
        return new Move(nextState) {
            IsCapture = isCapture,
            coordsBefore = Coordinates.FromMask(maskBefore),
            coordsAfter = Coordinates.FromMask(maskAfter)
        };
    }

}