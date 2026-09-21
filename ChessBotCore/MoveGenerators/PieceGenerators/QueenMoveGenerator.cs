using ChessBotCore.Board;

namespace ChessBotCore.MoveGenerators.PieceGenerators;

public sealed class QueenMoveGenerator : RayMoveGenerator, IGeneratorSingleton {
    private QueenMoveGenerator() { }

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

    public static IPieceMoveGenerator Instance => new QueenMoveGenerator();
}
