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

    public override void GenerateMoves(State state, List<Move> buffer) {
        GenerateCaptures(state, buffer);
        GenerateNonCaptures(state, buffer);
    }

    private static void AddMoveHandlingPromotion(Move move, bool white, List<Move> buffer) {
        int to = move.To;
        int toRank = to / 8;
        bool isPromotionRank = toRank == 0 || toRank == 7;

        if (!isPromotionRank) {
            buffer.Add(move);
            return;
        }

        Pieces pieceType = white ? Pieces.WhitePawns : Pieces.BlackPawns;
        MoveFlags baseFlags = move.Flags | MoveFlags.Promotion;
        buffer.Add(new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToQueen));
        buffer.Add(new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToRook));
        buffer.Add(new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToBishop));
        buffer.Add(new Move(move.From, to, pieceType, baseFlags | MoveFlags.PromoteToKnight));
    }

    internal void GenerateNonCaptures(State state, List<Move> buffer) {
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
            
            AddMoveHandlingPromotion(new Move(from, to, pieceType, MoveFlags.None), whitesMove, buffer);
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
            AddMoveHandlingPromotion(new Move(from, to, pieceType, MoveFlags.DoublePawnPush), whitesMove, buffer);
            movedTwice &= ~toMask;
        }
    }

    internal void GenerateCaptures(State state, List<Move> buffer) {
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

                AddMoveHandlingPromotion(new Move(from, to, pieceType, MoveFlags.Capture), state.WhiteIsActive, buffer);
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
                    AddMoveHandlingPromotion(
                        new Move(from, to, pieceType, MoveFlags.Capture | MoveFlags.EnPassant),
                        state.WhiteIsActive,
                        buffer);
                    pawnsThatCanCaptureEP &= ~fromMask;
                }
            }
        }
    }
}
