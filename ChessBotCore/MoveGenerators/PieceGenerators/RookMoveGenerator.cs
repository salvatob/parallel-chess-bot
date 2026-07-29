using ChessBotCore.Board;

namespace ChessBotCore.MoveGenerators.PieceGenerators;

public sealed class RookMoveGenerator : RayMoveGenerator, IGeneratorSingleton {
    private RookMoveGenerator() { }

    protected override Direction[] RayDirections => [
        Direction.E,
        Direction.N,
        Direction.S,
        Direction.W
    ];

    protected override Pieces WhitePiece => Pieces.WhiteRooks;
    protected override Pieces BlackPiece => Pieces.BlackRooks;

    public static IMoveGenerator Instance => new RookMoveGenerator();
}
