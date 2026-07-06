namespace ChessBotCore;

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
        // var enemyPieces = state.GetInactivePieces();
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
    }

    //TODO implement castling logic
    private IEnumerable<Move> GenerateCastleMoves(State state) {
        return [];
    }
}