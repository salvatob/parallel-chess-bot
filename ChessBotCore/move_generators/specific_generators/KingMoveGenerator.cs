namespace ChessBotCore;

public sealed class KingMoveGenerator : MoveGeneratorBase, IGeneratorSingleton {
    private static Direction[] MoveDirections = [
        Direction.N,
        Direction.S,
        Direction.E,
        Direction.W,
        Direction.NE,
        Direction.NW,
        Direction.SE,
        Direction.SW
    ];
    
    protected override Pieces WhitePiece => Pieces.WhiteKing;
    protected override Pieces BlackPiece => Pieces.BlackKing;


    private KingMoveGenerator(){}
    public static IMoveGenerator Instance => new KingMoveGenerator();
    
    public override IEnumerable<Move> GenerateMoves(State state) {
        var king = state.WhiteIsActive ? state.WhiteKing : state.BlackKing;
        var allyPieces = state.GetActivePieces();
        // var enemyPieces = state.GetInactivePieces();
        foreach (var dir in MoveDirections) {
            var beforeCollision = king.MovePieces(dir);
            var movedKing = beforeCollision & (~allyPieces);

            
            Direction oppositeDir = BitBoardHelpers.OppositeDir(dir);
            // foreach moved king
            while (movedKing.RawBits != 0) {
                // select one new king position
                int currMoved = movedKing.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = currMoveMask.MovePieces(oppositeDir);

                // according to old and new positions create the new State 
                Move newMove = CreateMove(maskBefore, currMoveMask, state);
                
                // disallow castling for current color
                newMove = HandleCastling(newMove);
                yield return newMove;


                movedKing &= ~currMoveMask;
            }
            //TODO implement castling logic
        }
    }

    private Move HandleCastling(Move move) {
        bool white = !move.StateAfter.WhiteIsActive;
        if (white) {
            move.StateAfter = move.StateAfter with { WhiteCastleKingSide = false, WhiteCastleQueenSide = false };
        } else {
            move.StateAfter = move.StateAfter with { BlackCastleKingSide = false, BlackCastleQueenSide = false };
        }

        return move;
    }
}