namespace ChessBotCore;

public class QueenMoveGenerator : RayMoveGenerator, IGeneratorSingleton {
    
    protected override Pieces WhitePiece => Pieces.WhiteQueens;
    protected override Pieces BlackPiece => Pieces.BlackQueens;

    protected override Direction[] RayDirections => [
        Direction.NW,
        Direction.NE,
        Direction.SW,
        Direction.SE
    ];

    private QueenMoveGenerator(){}
    
    public static IMoveGenerator Instance => new QueenMoveGenerator();
    
}