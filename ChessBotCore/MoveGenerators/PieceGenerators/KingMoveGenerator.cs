using ChessBotCore.Board;

namespace ChessBotCore.MoveGenerators.PieceGenerators;

public sealed class KingMoveGenerator : MoveGeneratorBase, IGeneratorSingleton {
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
    
    protected override Pieces WhitePiece => Pieces.WhiteKing;
    protected override Pieces BlackPiece => Pieces.BlackKing;


    private KingMoveGenerator(){}
    public static IMoveGenerator Instance => new KingMoveGenerator();
    
    public override void GenerateMoves(State state, List<Move> buffer) {
        var king = state.WhiteIsActive ? state.WhiteKing : state.BlackKing;
        var allyPieces = state.GetActivePieces();
        foreach (var dir in MoveDirections) {
            var beforeCollision = king.MovePieces(dir);
            var movedKing = beforeCollision & (~allyPieces);

            
            Direction oppositeDir = BitBoardHelpers.OppositeDir(dir);
            // foreach moved king
            while (movedKing.RawBits != 0) {
                // select one new king position
                int currMoved = movedKing.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = currMoveMask.MovePieces(oppositeDir);

                // according to old and new positions create the new State 
                buffer.Add(CreateMove(maskBefore, currMoveMask, state));

                movedKing &= ~currMoveMask;
            }
        }

        GenerateCastleMoves(state, buffer);
    }

    private void GenerateCastleMoves(State state, List<Move> buffer) {
        if (state.WhiteIsActive) {
            if (CanCastle(state, kingFrom: 3, rookFrom: 0, emptySquares: [2, 1], safeSquares: [3, 2, 1])) {
                var newMove = new Move(3, 1, Pieces.WhiteKing, MoveFlags.KingCastle);
                buffer.Add(newMove);
            }

            if (CanCastle(state, kingFrom: 3, rookFrom: 7, emptySquares: [4, 5, 6], safeSquares: [3, 4, 5])) {
                var newMove = new Move(3, 5, Pieces.WhiteKing, MoveFlags.QueenCastle);
                buffer.Add(newMove);
            }
        } else {
            if (CanCastle(state, kingFrom: 59, rookFrom: 56, emptySquares: [58, 57], safeSquares: [59, 58, 57])) {
                var newMove = new Move(59, 57, Pieces.BlackKing, MoveFlags.KingCastle);
                buffer.Add(newMove);
            }

            if (CanCastle(state, kingFrom: 59, rookFrom: 63, emptySquares: [60, 61, 62], safeSquares: [59, 60, 61])) {
                var newMove = new Move(59, 61, Pieces.BlackKing, MoveFlags.QueenCastle);
                buffer.Add(newMove);
            }
        }
    }

    private static bool CanCastle(State state, int kingFrom, int rookFrom, int[] emptySquares, int[] safeSquares) {
        bool kingSide = rookFrom is 0 or 56;

        if (state.WhiteIsActive) {
            if (kingSide && !state.WhiteCastleKingSide) return false;
            if (!kingSide && !state.WhiteCastleQueenSide) return false;
            if ((state.WhiteKing & BitBoardHelpers.OneBitMask(kingFrom)).IsEmpty()) return false;
            if ((state.WhiteRooks & BitBoardHelpers.OneBitMask(rookFrom)).IsEmpty()) return false;
        } else {
            if (kingSide && !state.BlackCastleKingSide) return false;
            if (!kingSide && !state.BlackCastleQueenSide) return false;
            if ((state.BlackKing & BitBoardHelpers.OneBitMask(kingFrom)).IsEmpty()) return false;
            if ((state.BlackRooks & BitBoardHelpers.OneBitMask(rookFrom)).IsEmpty()) return false;
        }

        Bitboard occupied = state.GetAllPieces();
        foreach (int square in emptySquares) {
            if (!(occupied & BitBoardHelpers.OneBitMask(square)).IsEmpty()) return false;
        }

        bool attackedByWhite = !state.WhiteIsActive;
        foreach (int square in safeSquares) {
            if (GeneratorWrapper.IsSquareAttacked(square, attackedByWhite, state)) return false;
        }

        return true;
    }
}
