namespace ChessBotCore;

public enum KnightDriections {
    NNE,NEE,NNW,NWW,
    SSE,SEE,SSW,SWW
}

public class KnightMoveGenerator : IMoveGenerator {
    private static KnightDriections[] MoveDirections = {
        KnightDriections.NNE,
        KnightDriections.NEE,
        KnightDriections.SEE,
        KnightDriections.SSE,
        KnightDriections.NNW,
        KnightDriections.NWW,
        KnightDriections.SWW,
        KnightDriections.SSW
    };
    
    public static List<State> GenerateMoves(State state) {
        throw new NotImplementedException();
    }

    public List<Move> GenarateMovePositions(State state) {
        List<Move> possibleMoves = [];
        var knights = state.WhiteIsActive ? state.WhiteKnights : state.BlackKnights;
        var allyPieces = state.GetActivePieces();
        var enemyPieces = state.GetInactivePieces();
        
        foreach (var dir in MoveDirections) {
            var beforeCollision = Move(knights, dir);
            var movedKnights = beforeCollision & allyPieces;
            

            // var splitIntoMoves = SplitIntoMoves(knights, movedKnights, dir);

            KnightDriections oppositeDir = OppositeDir(dir);
            // foreach moved knight
            while (movedKnights.RawBits != 0) {
                int currMoved = movedKnights.TrailingZeroCount();

                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = ~(Move(currMoveMask, oppositeDir));

                
                Bitboard newKnights = (knights | currMoveMask) & maskBefore;

                movedKnights &= ~currMoveMask;
            }


            return possibleMoves;
        }

    }

    private Move CreateMove(Bitboard maskBefore, Bitboard maskAfter, State state) {
        bool white = state.WhiteIsActive;
        var capture = state.DetectPieces(maskAfter);
        bool isCapture = !capture.HasValue;
        if (white) {
            Bitboard nextWhiteKnights = ((state.WhiteKnights | maskAfter) & (~maskBefore));
            if (!isCapture) {
                return new Move(
                    state.Next() with {
                        WhiteKnights = nextWhiteKnights
                    }
                );
            }

            var newCapturedPieces = state.GetPieces(capture!.Value) & ~maskAfter;
            return new Move(
                (state.Next() with {
                    WhiteKnights = nextWhiteKnights
                }).With(capture!.Value, newCapturedPieces)
            );
        }
        
    }
    
    public static List<Bitboard> SplitIntoMoves(Bitboard before, Bitboard after, KnightDriections dir) {
        List<Bitboard> result = [];
        KnightDriections oppositeDir = OppositeDir(dir);
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

    private static Bitboard Move(Bitboard bits, KnightDriections dir) {
        Bitboard withoutLeftCol = ~ BitMask.Col[0];
        Bitboard withoutRightCol = ~ BitMask.Col[7];
        Bitboard withoutTwoLeftCols = ~ (BitMask.Col[0] | BitMask.Col[1]);
        Bitboard withoutTwoRightCols = ~ (BitMask.Col[6] | BitMask.Col[7]);
            
        return dir switch {
            KnightDriections.NNE =>  (bits << 17) & withoutLeftCol ,
            KnightDriections.NEE =>  (bits << 10) & withoutTwoLeftCols,
            KnightDriections.SEE =>  (bits >>  6) & withoutTwoLeftCols,
            KnightDriections.SSE =>  (bits >> 15) & withoutLeftCol ,
            KnightDriections.NNW =>  (bits << 15) & withoutRightCol ,
            KnightDriections.NWW =>  (bits <<  6) & withoutTwoRightCols,
            KnightDriections.SWW =>  (bits >> 10) & withoutTwoRightCols,
            KnightDriections.SSW =>  (bits >> 17) & withoutRightCol ,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }

    private static KnightDriections OppositeDir(KnightDriections dir) {
        return dir switch {
            KnightDriections.SSW => KnightDriections.NNE,
            KnightDriections.SWW => KnightDriections.NEE,
            KnightDriections.NWW => KnightDriections.SEE,
            KnightDriections.NNW => KnightDriections.SSE,
            KnightDriections.SSE => KnightDriections.NNW,
            KnightDriections.SEE => KnightDriections.NWW,
            KnightDriections.NEE => KnightDriections.SWW,
            KnightDriections.NNE => KnightDriections.SSW,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
}