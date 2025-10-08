using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChessBotCore;

public sealed class PawnMoveGenerator : MoveGeneratorBase, IMoveGenerator {
    
    private PawnMoveGenerator(){}
    public static IMoveGenerator Instance => new PawnMoveGenerator();

    protected override Pieces WhitePiece => Pieces.WhitePawns;
    protected override Pieces BlackPiece => Pieces.BlackPawns;

    private Direction _whiteForward = Direction.N;
    private Direction _blackForward = Direction.S;

    private Direction[] _whiteDiagonals = [Direction.NW, Direction.NE];
    private Direction[] _blackDiagonals = [Direction.SW, Direction.SE];
    
    

    public override IEnumerable<Move> GenerateMoves(State state) {
        return HandlePromotions(
            GenerateWithoutPromotions(state)
        );
    }

    /// <summary>
    /// Generates all moves, but doesn't handle promotions.
    /// </summary>
    /// <param name="state">The state, from which new moves are generated</param>
    /// <returns>IEnumerable of all new Moves.</returns>
    internal IEnumerable<Move> GenerateWithoutPromotions(State state) {
        foreach (Move move in GenerateCaptures(state)) {
            yield return move;
        }

        foreach (Move move in GenerateNonCaptures(state)) {
            yield return move;
        }
    }

    internal IEnumerable<Move> GenerateNonCaptures(State state) {
        bool whitesMove = state.WhiteIsActive;
        var currentPawns = whitesMove ? state.WhitePawns : state.BlackPawns;
        Bitboard emptySpace = ~state.GetAllPieces();
        var currentPieces = whitesMove ? Pieces.WhitePawns : Pieces.BlackPawns;
        
        
        // foreach standard move forward
        foreach (var board in OneCellForward(currentPawns, emptySpace, whitesMove)) {
            yield return SetupNewMove(board);
        }
        // foreach double move forward
        foreach (var board in DoubleMoveForward(currentPawns, emptySpace, whitesMove)) {
            var enPassantMask = FindEnpassant(whitesMove, currentPawns, board);
            
            Move newMove = SetupNewMove(board);
            newMove.StateAfter = newMove.StateAfter with { EnPassant = enPassantMask };
            yield return newMove;
        }
        
        
        
        Move SetupNewMove(Bitboard board) {
            State newState  = 
                state.Next()
                    .WithHalfClockReset().
                    With(currentPieces, board);
            
            Move newMove = new Move(newState) {IsCapture = false};

            return newMove;
        }
    }

    private Bitboard FindEnpassant(bool white, Bitboard pawnsBefore, Bitboard pawnsAfter) {
        Direction dirBackward = white ? _blackForward : _whiteForward; 

        // represents the piece after moving
        Bitboard movemask = (pawnsBefore ^ pawnsAfter) & (~ pawnsBefore);
        Debug.Assert(movemask.PopCount() == 1);

        // enpassant square is defined as the square the pawn hopped ove, so move the mask one square back
        return movemask.MovePieces(dirBackward);
    }
    
    //TODO implement enpassant
    
    /// <summary>
    /// Wrapper method, that will expand promotion moves to all four possibilites.
    /// </summary>
    /// <param name="moves">IEnumerable of the moves that have a pawn in the last row.</param>
    /// <returns>IEnumerable of the new moves, with all promotion options accounted.</returns>
    internal IEnumerable<Move> HandlePromotions(IEnumerable<Move> moves) {
        foreach (Move move in moves) {
            var state = move.StateAfter;
            bool whitesMove = !state.WhiteIsActive;
            var lastRow = BitMask.Row[whitesMove ? 7 : 0];
            var pawns = whitesMove ? state.WhitePawns : state.BlackPawns;

            var promotionMask = pawns & lastRow;
            Debug.Assert(promotionMask.PopCount() <= 1,
                "More than one pawn promotion at once is not possible");

            // means the move is not promotion
            if (promotionMask.IsEmpty()) {
                yield return move;
                
            } else {
                var newPawns = pawns & (~lastRow);

                Pieces[] piecesToPromoteTo = whitesMove
                    ? [Pieces.WhiteQueens, Pieces.WhiteKnights, Pieces.WhiteRooks, Pieces.WhiteBishops]
                    : [Pieces.BlackQueens, Pieces.BlackKnights, Pieces.BlackRooks, Pieces.BlackBishops];


                Pieces currentPawns = whitesMove ? Pieces.WhitePawns : Pieces.BlackPawns;
                // create new queen, knight etc.
                foreach (Pieces piece in piecesToPromoteTo) {
                    Bitboard newPieces = state.GetPieces(piece) | promotionMask;
                    State newState = state.With(piece, newPieces).With(currentPawns, newPawns);
            
                    yield return move with {IsPromotion = true, StateAfter = newState};
                }
        
            }
        }
    }
    
    internal IEnumerable<Bitboard> OneCellForward(Bitboard pawns, Bitboard emptyCells, bool colorWhite) {
        Direction dirForward = colorWhite ? _whiteForward : _blackForward;
        Direction oppositeDirection = BitBoardHelpers.OppositeDir(dirForward);
        
        // moves all pieces one step forward
        // this variable represents new positions, deleting all that would result in a collision
        Bitboard moved = (pawns.MovePieces(dirForward)) & (emptyCells);
        
        while (!moved.IsEmpty()) {
            int currMoved = moved.TrailingZeroCount();
            Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            Bitboard emptySpaceAfterMoving = ~currMoveMask.MovePieces(oppositeDirection);

            Bitboard newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            yield return newPawns;

            moved &= ~currMoveMask;
        }

    }

    internal IEnumerable<Bitboard> DoubleMoveForward(Bitboard pawns, Bitboard emptyCells, bool colorWhite) {
        
        Direction dirForward = colorWhite ? _whiteForward : _blackForward;
        Direction oppositeDirection = BitBoardHelpers.OppositeDir(dirForward);
        
        // mask of all pawns, that are on the starting row of the respective color, since only there pawns can double jump
        Bitboard startingPawns = pawns & (colorWhite ? BitMask.Row[1] : BitMask.Row[6]);
        
        // moves all pieces two steps forward
        // this variable represents new positions, deleting all that would result in a collision
        Bitboard moved = BitBoardHelpers.Move(startingPawns, dirForward, 2) & emptyCells;
            
        while (!moved.IsEmpty()) {
            int currMoved = moved.TrailingZeroCount();
            Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

            Bitboard emptySpaceAfterMoving = ~currMoveMask.MovePieces(oppositeDirection).MovePieces(oppositeDirection);

            Bitboard newPawns = (pawns | currMoveMask) & emptySpaceAfterMoving;
            
            // TODO make this double move set enpassant possibility to the Move struct
            yield return newPawns;

            moved &= ~currMoveMask;
        }
    }

    internal IEnumerable<Move> GenerateCaptures(State state) {
        var enemyPieces = state.GetInactivePieces();
        var pawns = state.WhiteIsActive ? state.WhitePawns : state.BlackPawns;
        var directions = state.WhiteIsActive ? _whiteDiagonals : _blackDiagonals;

        foreach (Direction dir in directions) {
            var pawnsThatCapturedSomething = pawns.MovePieces(dir) & enemyPieces;

            Direction oppositeDir = BitBoardHelpers.OppositeDir(dir);
            // foreach moved pawn
            while (pawnsThatCapturedSomething.RawBits != 0) {
                // select one new pawn position
                int currMoved = pawnsThatCapturedSomething.TrailingZeroCount();

                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);

                Bitboard maskBefore = BitBoardHelpers.Move(currMoveMask, oppositeDir);

                // according to old and new positions create the new State 
                yield return CreateMove(maskBefore, currMoveMask, state);

                pawnsThatCapturedSomething &= ~currMoveMask;
            }
        }
    }
}