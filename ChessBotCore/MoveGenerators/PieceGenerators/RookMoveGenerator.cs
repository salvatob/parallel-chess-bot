using ChessBotCore.Board;

namespace ChessBotCore.MoveGenerators.PieceGenerators;

public sealed class RookMoveGenerator : RayMoveGenerator, IGeneratorSingleton {

    public static IMoveGenerator Instance => new RookMoveGenerator();
    private RookMoveGenerator(){}
    
    protected override Direction[] RayDirections => [
        Direction.E,
        Direction.N,
        Direction.S,
        Direction.W
    ];

    protected override Pieces WhitePiece => Pieces.WhiteRooks;
    protected override Pieces BlackPiece => Pieces.BlackRooks;
}
