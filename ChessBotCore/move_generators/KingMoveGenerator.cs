namespace ChessBotCore;

public class KingMoveGenerator : IMoveGenerator {
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
    

    public static List<Move> GenerateMoves(State state) {
        List<Move> possibleMoves = [];
        var king = state.WhiteIsActive ? state.WhiteKing : state.BlackKing;
        var allyPieces = state.GetActivePieces();
        // var enemyPieces = state.GetInactivePieces();
#if DEBUG
        Console.WriteLine("king");
        Console.WriteLine(king);
#endif
        
        foreach (var dir in MoveDirections) {
            var beforeCollision = king.MovePieces(dir);
            var movedKing = beforeCollision & (~allyPieces);

#if DEBUG

            Console.WriteLine($"movedKnights in dir {dir}");
            Console.WriteLine(movedKing);
#endif
            
            // var splitIntoMoves = SplitIntoMoves(knights, movedKnights, dir);

            Direction oppositeDir = OppositeDir(dir);
            // foreach moved knight
            while (movedKing.RawBits != 0) {
                // select one new knight position
                int currMoved = movedKing.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = currMoveMask.MovePieces(oppositeDir);
                
                // according to old and new positions create the new State 
                possibleMoves.Add(CreateMove(maskBefore, currMoveMask, state));                

                movedKing &= ~currMoveMask;
            }
        }
        
        return possibleMoves;

    }

    private static Move CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
        bool white = state.WhiteIsActive;
        var capture = state.DetectPieces(maskAfter);

        State nextState;
        Bitboard nextKing;
        // adds the moved knight, removes the before knight
        if (white) {
            nextKing = ((state.WhiteKing | maskAfter) & (~maskBefore));

            nextState = state.Next() with { WhiteKing = nextKing };
        } else {
            nextKing = ((state.WhiteKing | maskAfter) & (~maskBefore));

            nextState = state.Next() with { WhiteKing = nextKing };
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
    
    
    private static Direction OppositeDir(Direction dir) {
        return dir switch {
            Direction.N => Direction.S,
            Direction.S => Direction.N,
            Direction.W => Direction.E,
            Direction.E => Direction.W,
            Direction.NE => Direction.SW,
            Direction.NW => Direction.SE,
            Direction.SE => Direction.NW,
            Direction.SW => Direction.NE,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
}