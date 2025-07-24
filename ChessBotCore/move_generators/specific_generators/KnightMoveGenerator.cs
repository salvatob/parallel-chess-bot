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
        // var enemyPieces = state.GetInactivePieces();
#if DEBUG
        Console.WriteLine("knights");
        Console.WriteLine(knights);
#endif
        
        foreach (var dir in KnightDirections) {
            var beforeCollision = BitBoardHelpers.Move(knights, dir);
            var movedKnights = beforeCollision & (~allyPieces);

#if DEBUG

            Console.WriteLine($"movedKnights in dir {dir}");
            Console.WriteLine(movedKnights);
#endif
            
            // var splitIntoMoves = SplitIntoMoves(knights, movedKnights, dir);

            Direction oppositeDir =BitBoardHelpers. OppositeDir(dir);
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

    private new Move  CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
        bool white = state.WhiteIsActive;
        var capture = state.DetectPieces(maskAfter);

        State nextState;
        Bitboard nextKnights;
        // adds the moved knight, removes the before knight
        if (white) {
            nextKnights = ((state.WhiteKnights | maskAfter) & (~maskBefore));

            nextState = state.Next() with { WhiteKnights = nextKnights };
        } else {
            nextKnights = ((state.BlackKnights | maskAfter) & (~maskBefore));

            nextState = state.Next() with { BlackKnights = nextKnights };
        }
        // additionally removes the captured piece from its corresponding bitboard 
        bool isCapture = capture.HasValue;
        if (isCapture) {
            var newCapturedPieces = state.GetPieces(capture.Value) & ~maskAfter;
            nextState = nextState.With(capture.Value, newCapturedPieces);
        }

        return new Move(nextState) {
            IsCapture = isCapture
        };
    }
    

}