namespace ChessBotCore;

public sealed class RookMoveGenerator : RayMoveGenerator, IGeneratorSingleton {

    public static IMoveGenerator Instance => new RookMoveGenerator();
    private RookMoveGenerator(){}
    
    protected override Direction[] RayDirections => [
        Direction.E,
        Direction.N,
        Direction.S,
        Direction.W
    ];

    //TODO
    protected override Pieces WhitePiece => Pieces.WhiteRooks;
    protected override Pieces BlackPiece => Pieces.BlackRooks;
}