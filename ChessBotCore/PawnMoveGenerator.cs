using System.Numerics;

namespace ChessBotCore;

public interface IMoveGenerator {
    public static abstract List<State> GenerateMoves(State state);
} 

public class PawnMoveGenerator : IMoveGenerator {
    public static List<State> GenerateMoves(State state) {
        throw new NotImplementedException();
    }

    public static List<ulong> OneCellForward(ulong pawns, ulong emptyCells) {
        List<ulong> newPawnBoards = [];
        // moves all pieces one step forward
        // this variable represents new positions, deleting all that would result in a collision
        ulong moved = (pawns << 8) & (emptyCells);
        
        while (moved != 0) {
            int currMoved = BitOperations.TrailingZeroCount(moved);
            ulong currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            ulong emptySpaceAfterMoving = ~(currMoveMask >> 8);

            ulong newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            newPawnBoards.Add(newPawns);

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }

    public static List<ulong> DoubleMoveForward(ulong pawns, ulong emptyCells) {
        List<ulong> newPawnBoards = [];
        // moves all pieces two steps forward
        // this variable represents new positions, deleting all that would result in a collision
        ulong startingPawns = pawns & BitMask.Row[1];
        ulong movedOneCell = (startingPawns << 8) & (emptyCells);
        ulong moved = (movedOneCell << 8) & (emptyCells);

#if DEBUG
        Console.WriteLine("--pawns--");
        Console.WriteLine(pawns.PrintAsBitBoard());
        Console.WriteLine("--emptyCells--");
        Console.WriteLine(emptyCells.PrintAsBitBoard());
        Console.WriteLine("--startingPawns--");
        Console.WriteLine(startingPawns.PrintAsBitBoard());
        Console.WriteLine("--movedOneCell--");
        Console.WriteLine(movedOneCell.PrintAsBitBoard());
        Console.WriteLine("--moved--");
        Console.WriteLine(moved.PrintAsBitBoard());
#endif        
        
        while (moved != 0) {
            int currMoved = BitOperations.TrailingZeroCount(moved);
            ulong currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            ulong emptySpaceAfterMoving = ~(currMoveMask >> 16);

            ulong newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            newPawnBoards.Add(newPawns);

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }
}