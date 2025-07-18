using System.Numerics;

namespace ChessBotCore;

public interface IMoveGenerator {
    public static abstract List<State> GenerateMoves(State state);
} 

public class PawnMoveGenerator : IMoveGenerator {
    public static List<State> GenerateMoves(State state) {
        throw new NotImplementedException();
    }

    public static List<ulong> OneCellForward(ulong pawns, ulong allOtherPieces) {
        List<ulong> newPawnBoards = [];
        ulong moved = pawns << 8;
        
        while (moved != 0) {
            int currMoved = BitOperations.TrailingZeroCount(moved);
            ulong currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            // pawn forward move cannot collide with anything
            if ((currMoveMask & allOtherPieces) == 0) {
                ulong emptySpaceAfterMoving = ~(currMoveMask << 8);

                ulong newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
                newPawnBoards.Add(newPawns);
            }

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }

    public static List<ulong> DoubleMove(ulong pawns, ulong allOtherPieces) {
        List<ulong> newPawnBoards = [];
        // moves all pieces one step forward
        // this variable represents new positions, deleting all that would result in a collision
        ulong moved = (pawns << 8) & (~allOtherPieces);
        
        while (moved != 0) {
            int currMoved = BitOperations.TrailingZeroCount(moved);
            ulong currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            ulong emptySpaceAfterMoving = ~(currMoveMask << 8);

            ulong newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            newPawnBoards.Add(newPawns);

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }
}