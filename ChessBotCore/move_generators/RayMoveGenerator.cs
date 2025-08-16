namespace ChessBotCore;

/// <summary>
/// Implements most of the logic fom move generation of ray pieces (bishop, rook, queen)
/// </summary>
public abstract class RayMoveGenerator : MoveGeneratorBase {

    
    protected abstract Direction[] RayDirections { get; }

    
    public override IEnumerable<Move> GenerateMoves(State state) {
        var rayPiece = state.GetPieces(state.WhiteIsActive ? WhitePiece : BlackPiece);
        var allyPieces = state.GetActivePieces();
        var enemyPieces = state.GetInactivePieces();
        var allPieces = state.GetAllPieces();
        
        foreach (var dir in RayDirections) {
            Bitboard  moveMask = BitBoardHelpers.Move(rayPiece, dir);
            var oppositeDir = BitBoardHelpers.OppositeDir(dir);

            int distance = 0;
            // send all pieces as rays as far as possible
            while (!moveMask.IsEmpty()) {
                distance++;
                
                // delete collisions with enemy pieces
                var withoutCollisions = moveMask & ( ~allyPieces);
                var currentMoveCopy = withoutCollisions;
                // split each moved piece into its own move
                while (!currentMoveCopy.IsEmpty()) {
                    var currMoved = currentMoveCopy.TrailingZeroCount();
                    
                    Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                    Bitboard maskBefore = BitBoardHelpers.Move(currMoveMask, oppositeDir, distance);
                
                    // according to old and new positions create the new State 
                    yield return CreateMove(maskBefore, currMoveMask, state);                

                    currentMoveCopy &= ~currMoveMask;
                }

                // collisions with enemy pieces are legal, however the ray should there, so I delete such pieces
                var withoutCaptures = withoutCollisions & (~enemyPieces);
                moveMask = BitBoardHelpers.Move(withoutCaptures, dir);
            }
            
        }

    }

}