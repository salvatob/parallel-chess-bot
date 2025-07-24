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
    
    public override IEnumerable<Move> GenerateMoves(State state) {
        var king = state.WhiteIsActive ? state.WhiteKing : state.BlackKing;
        var allyPieces = state.GetActivePieces();
        // var enemyPieces = state.GetInactivePieces();
#if DEBUG
        Console.WriteLine("king");
        Console.WriteLine(king);
#endif
        
        foreach (var dir in MoveDirections) {
            var beforeCollision = king.MovePieces(dir);
            var movedKing = beforeCollision & (~allyPieces);

#if DEBUG

            Console.WriteLine($"movedKnights in dir {dir}");
            Console.WriteLine(movedKing);
#endif
            
            // var splitIntoMoves = SplitIntoMoves(knights, movedKnights, dir);

            Direction oppositeDir = BitBoardHelpers.OppositeDir(dir);
            // foreach moved knight
            while (movedKing.RawBits != 0) {
                // select one new knight position
                int currMoved = movedKing.TrailingZeroCount();
                
                Bitboard currMoveMask = BitBoardHelpers.OneBitMask(currMoved);
                
                Bitboard maskBefore = currMoveMask.MovePieces(oppositeDir);

                // according to old and new positions create the new State 
                yield return CreateMove(maskBefore, currMoveMask, state);


                movedKing &= ~currMoveMask;
            }
        }
    }
}