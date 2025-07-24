namespace ChessBotCore;

public class BishopMoveGenerator : RayMoveGenerator, IGeneratorSingleton {
    protected override Pieces WhitePiece => Pieces.WhiteBishops;
    protected override Pieces BlackPiece => Pieces.BlackBishops;

    protected override Direction[] RayDirections => [
        Direction.NW,
        Direction.NE,
        Direction.SW,
        Direction.SE
    ];

    private BishopMoveGenerator(){}
    
    public static IMoveGenerator Instance => new BishopMoveGenerator();
}