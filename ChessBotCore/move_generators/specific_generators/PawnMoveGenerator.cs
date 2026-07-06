using System.Collections.Generic;

namespace ChessBotCore;

public sealed class PawnMoveGenerator : MoveGeneratorBase, IMoveGenerator {
    
    private PawnMoveGenerator(){}
    public static IMoveGenerator Instance => new PawnMoveGenerator();

    protected override Pieces WhitePiece => Pieces.WhitePawns;
    protected override Pieces BlackPiece => Pieces.BlackPawns;

    private readonly Direction _whiteForward = Direction.N;
    private readonly Direction _blackForward = Direction.S;

    private readonly Direction[] _whiteDiagonals = [Direction.NW, Direction.NE];
    private readonly Direction[] _blackDiagonals = [Direction.SW, Direction.SE];

    public override IEnumerable<Move> GenerateMoves(State state) {
        foreach (Move move in GenerateCaptures(state)) {
            foreach (Move promoted in HandlePromotion(move, state.WhiteIsActive))
                yield return promoted;
        }

        foreach (Move move in GenerateNonCaptures(state)) {
            foreach (Move promoted in HandlePromotion(move, state.WhiteIsActive))
                yield return promoted;
        }
    }

    private IEnumerable<Move> HandlePromotion(Move move, bool white) {
        int to = move.To;
        int toRank = to / 8;
        bool isPromotionRank = toRank == 0 || toRank == 7;

        if (!isPromotionRank) {
            yield return move;
        } else {
            Pieces pieceType = white ? Pieces.WhitePawns : Pieces.BlackPawns;
            MoveFlags baseFlags = move.Flags | MoveFlags.Promotion;
            yield return new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToQueen);
            yield return new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToRook);
            yield return new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToBishop);
            yield return new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToKnight);
        }
    }

    internal IEnumerable<Move> GenerateNonCaptures(State state) {
        bool whitesMove = state.WhiteIsActive;
        Pieces pieceType = whitesMove ? Pieces.WhitePawns : Pieces.BlackPawns;
        
        Bitboard currentPawns = whitesMove ? state.WhitePawns : state.BlackPawns;
        Bitboard emptySpace = ~state.GetAllPieces();

        Direction dirForward = whitesMove ? _whiteForward : _blackForward;
        Direction oppositeDir = BitBoardHelpers.OppositeDir(dirForward);

        // One cell forward
        Bitboard movedOnce = currentPawns.MovePieces(dirForward) & emptySpace;
        Bitboard tempOnce = movedOnce;
        while (!tempOnce.IsEmpty()) {
            int to = tempOnce.TrailingZeroCount();
            Bitboard toMask = BitBoardHelpers.OneBitMask(to);
            int from = BitBoardHelpers.Move(toMask, oppositeDir).TrailingZeroCount();
            
            yield return new Move(from, to, pieceType, MoveFlags.None);
            tempOnce &= ~toMask;
        }

        // Double move forward
        Bitboard startingPawns = currentPawns & (whitesMove ? BitMask.Row[1] : BitMask.Row[6]);
        Bitboard movedTwice = BitBoardHelpers.Move(startingPawns, dirForward, 1) & emptySpace;
        movedTwice = BitBoardHelpers.Move(movedTwice, dirForward, 1) & emptySpace;
        
        while (!movedTwice.IsEmpty()) {
            int to = movedTwice.TrailingZeroCount();
            Bitboard toMask = BitBoardHelpers.OneBitMask(to);
            int from = BitBoardHelpers.Move(toMask, oppositeDir, 2).TrailingZeroCount();
            yield return new Move(from, to, pieceType, MoveFlags.DoublePawnPush);
            movedTwice &= ~toMask;
        }
    }

    internal IEnumerable<Move> GenerateCaptures(State state) {
        Bitboard enemyPieces = state.GetInactivePieces();
        Bitboard pawns = state.WhiteIsActive ? state.WhitePawns : state.BlackPawns;
        Direction[] directions = state.WhiteIsActive ? _whiteDiagonals : _blackDiagonals;
        Pieces pieceType = state.WhiteIsActive ? Pieces.WhitePawns : Pieces.BlackPawns;
        
        
        foreach (Direction dir in directions) {
            var pawnsThatCapturedSomething = pawns.MovePieces(dir) & enemyPieces;
            var oppositeDir = BitBoardHelpers.OppositeDir(dir);

            while (!pawnsThatCapturedSomething.IsEmpty()) {
                int to = pawnsThatCapturedSomething.TrailingZeroCount();
                Bitboard toMask = BitBoardHelpers.OneBitMask(to);
                int from = BitBoardHelpers.Move(toMask, oppositeDir).TrailingZeroCount();

                yield return new Move(from, to, pieceType, MoveFlags.Capture);
                pawnsThatCapturedSomething &= ~toMask;
            }
        }
        
        // En passant
        if (state.EnPassantAvailable) {
            Bitboard epMask = state.EnPassant;
            foreach (Direction dir in directions) {
                // ReSharper disable once InconsistentNaming
                Bitboard pawnsThatCanCaptureEP = pawns & BitBoardHelpers.Move(epMask, BitBoardHelpers.OppositeDir(dir));
                
                while (!pawnsThatCanCaptureEP.IsEmpty()) {
                    int from = pawnsThatCanCaptureEP.TrailingZeroCount();
                    Bitboard fromMask = BitBoardHelpers.OneBitMask(from);
                    int to = BitBoardHelpers.Move(fromMask, dir).TrailingZeroCount();
                    yield return new Move(from, to, pieceType, MoveFlags.Capture | MoveFlags.EnPassant);
                    pawnsThatCanCaptureEP &= ~fromMask;
                }
            }
        }
    }
}
