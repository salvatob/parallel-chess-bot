namespace ChessBotCore;

public sealed class KnightMoveGenerator : MoveGeneratorBase, IGeneratorSingleton {
    private static Direction[] KnightDirections = [
        Direction.NNE,
        Direction.NEE,
        Direction.SEE,
        Direction.SSE,
        Direction.NNW,
        Direction.NWW,
        Direction.SWW,
        Direction.SSW
    ];
    
    protected override Pieces WhitePiece => Pieces.WhiteKnights;
    protected override Pieces BlackPiece => Pieces.BlackKnights;

    
    private KnightMoveGenerator(){}
    public static IMoveGenerator Instance => new KnightMoveGenerator();

    public override IEnumerable<Move> GenerateMoves(State state) {
        var knights = state.WhiteIsActive ? state.WhiteKnights : state.BlackKnights;
        var allyPieces = state.GetActivePieces();
        
        foreach (var dir in KnightDirections) {
            var beforeCollision = BitBoardHelpers.Move(knights, dir);
            var movedKnights = beforeCollision & (~allyPieces);
         

            Direction oppositeDir = BitBoardHelpers.OppositeDir(dir);
            // foreach moved knight
            while (movedKnights.RawBits != 0) {
                // select one new knight position
                int currMoved = movedKnights.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = BitBoardHelpers.Move(currMoveMask, oppositeDir);
                
                // according to old and new positions create the new State 
                yield return CreateMove(maskBefore, currMoveMask, state);                

                movedKnights &= ~currMoveMask;
            }
        }
        

    }

}