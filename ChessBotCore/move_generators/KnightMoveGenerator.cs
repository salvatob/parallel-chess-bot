namespace ChessBotCore;

public enum KnightDirections {
    NNE,NEE,NNW,NWW,
    SSE,SEE,SSW,SWW
}

public class KnightMoveGenerator : IMoveGenerator {
    private static KnightDirections[] MoveDirections = [
        KnightDirections.NNE,
        KnightDirections.NEE,
        KnightDirections.SEE,
        KnightDirections.SSE,
        KnightDirections.NNW,
        KnightDirections.NWW,
        KnightDirections.SWW,
        KnightDirections.SSW
    ];
    

    public static List<Move> GenerateMoves(State state) {
        List<Move> possibleMoves = [];
        var knights = state.WhiteIsActive ? state.WhiteKnights : state.BlackKnights;
        var allyPieces = state.GetActivePieces();
        // var enemyPieces = state.GetInactivePieces();
#if DEBUG
        Console.WriteLine("knights");
        Console.WriteLine(knights);
#endif
        
        foreach (var dir in MoveDirections) {
            var beforeCollision = Move(knights, dir);
            var movedKnights = beforeCollision & (~allyPieces);

#if DEBUG

            Console.WriteLine($"movedKnights in dir {dir}");
            Console.WriteLine(movedKnights);
#endif
            
            // var splitIntoMoves = SplitIntoMoves(knights, movedKnights, dir);

            KnightDirections oppositeDir = OppositeDir(dir);
            // foreach moved knight
            while (movedKnights.RawBits != 0) {
                // select one new knight position
                int currMoved = movedKnights.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = Move(currMoveMask, oppositeDir);
                
                // according to old and new positions create the new State 
                possibleMoves.Add(CreateMove(maskBefore, currMoveMask, state));                

                movedKnights &= ~currMoveMask;
            }
        }
        
        return possibleMoves;

    }

    private static Move CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
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
    
    public static List<Bitboard> SplitIntoMoves(Bitboard before, Bitboard after, KnightDirections dir) {
        List<Bitboard> result = [];
        KnightDirections oppositeDir = OppositeDir(dir);
        while (after.RawBits != 0) {
            int currMoved = after.TrailingZeroCount();

            Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
            Bitboard emptySpaceAfterMoving = ~(Move(currMoveMask, oppositeDir));

            Bitboard newKnights = (before | currMoveMask) & emptySpaceAfterMoving;
            result.Add(newKnights);

            after &= ~currMoveMask;
        }

        return result;
    }

    private static Bitboard Move(Bitboard bits, KnightDirections dir) {
        Bitboard withoutLeftCol = ~ BitMask.Col[0];
        Bitboard withoutRightCol = ~ BitMask.Col[7];
        Bitboard withoutTwoLeftCols = ~ (BitMask.Col[0] | BitMask.Col[1]);
        Bitboard withoutTwoRightCols = ~ (BitMask.Col[6] | BitMask.Col[7]);

#if DEBUG
        if (dir is KnightDirections.SEE or KnightDirections.NEE) {
            int _ = 4;
        }
#endif
        return dir switch {
            KnightDirections.NNE =>  (bits << 17) & withoutLeftCol ,
            KnightDirections.NEE =>  (bits << 10) & withoutTwoRightCols,
            KnightDirections.SEE =>  (bits >>  6) & withoutTwoRightCols,
            KnightDirections.SSE =>  (bits >> 15) & withoutLeftCol ,
            KnightDirections.NNW =>  (bits << 15) & withoutRightCol ,
            KnightDirections.NWW =>  (bits <<  6) & withoutTwoLeftCols,
            KnightDirections.SWW =>  (bits >> 10) & withoutTwoLeftCols,
            KnightDirections.SSW =>  (bits >> 17) & withoutRightCol ,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }

    private static KnightDirections OppositeDir(KnightDirections dir) {
        return dir switch {
            KnightDirections.SSW => KnightDirections.NNE,
            KnightDirections.SWW => KnightDirections.NEE,
            KnightDirections.NWW => KnightDirections.SEE,
            KnightDirections.NNW => KnightDirections.SSE,
            KnightDirections.SSE => KnightDirections.NNW,
            KnightDirections.SEE => KnightDirections.NWW,
            KnightDirections.NEE => KnightDirections.SWW,
            KnightDirections.NNE => KnightDirections.SSW,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
}