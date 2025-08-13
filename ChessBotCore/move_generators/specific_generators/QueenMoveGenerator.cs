namespace ChessBotCore;

public sealed class QueenMoveGenerator : RayMoveGenerator, IGeneratorSingleton {
    
    protected override Pieces WhitePiece => Pieces.WhiteQueens;
    protected override Pieces BlackPiece => Pieces.BlackQueens;

    protected override Direction[] RayDirections => [
        Direction.E,
        Direction.N,
        Direction.S,
        Direction.W,
        
        Direction.NW,
        Direction.NE,
        Direction.SW,
        Direction.SE
    ];

    private QueenMoveGenerator(){}
    
    public static IMoveGenerator Instance => new QueenMoveGenerator();
    
}