using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChessBotCore;

public class PawnMoveGenerator : MoveGeneratorBase, IMoveGenerator {
    
    private PawnMoveGenerator(){}
    public static IMoveGenerator Instance => new PawnMoveGenerator();

    protected override Pieces WhitePiece => Pieces.WhitePawns;
    protected override Pieces BlackPiece => Pieces.BlackPawns;

    public override IEnumerable<Move> GenerateMoves(State state) {
        throw new NotImplementedException();
    }

    internal List<Bitboard> OneCellForward(Bitboard pawns, Bitboard emptyCells) {
        List<Bitboard> newPawnBoards = [];
        // moves all pieces one step forward
        // this variable represents new positions, deleting all that would result in a collision
        Bitboard moved = (pawns << 8) & (emptyCells);
        
        while (moved.RawBits != 0) {
            int currMoved = moved.TrailingZeroCount();
            Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            Bitboard emptySpaceAfterMoving = ~(currMoveMask >> 8);

            Bitboard newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            newPawnBoards.Add(newPawns);

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }

    internal List<Bitboard> DoubleMoveForward(Bitboard pawns, Bitboard emptyCells) {
        List<Bitboard> newPawnBoards = [];
        // moves all pieces two steps forward
        // this variable represents new positions, deleting all that would result in a collision
        Bitboard startingPawns = pawns & BitMask.Row[1];
        Bitboard movedOneCell = (startingPawns << 8) & (emptyCells);
        Bitboard moved = (movedOneCell << 8) & (emptyCells);

#if DEBUG
        Console.WriteLine("--pawns--");
        Console.WriteLine(pawns.Print());
        Console.WriteLine("--emptyCells--");
        Console.WriteLine(emptyCells.Print());
        Console.WriteLine("--startingPawns--");
        Console.WriteLine(startingPawns.Print());
        Console.WriteLine("--movedOneCell--");
        Console.WriteLine(movedOneCell.Print());
        Console.WriteLine("--moved--");
        Console.WriteLine(moved.Print());
#endif        
        
        while (moved.RawBits != 0) {
            int currMoved = moved.TrailingZeroCount();
            Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            Bitboard emptySpaceAfterMoving = ~(currMoveMask >> 16);

            Bitboard newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            newPawnBoards.Add(newPawns);

            moved &= ~currMoveMask;
        }

        return newPawnBoards;
    }
}