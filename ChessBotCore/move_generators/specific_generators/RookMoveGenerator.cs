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

    // TODO castles forbidding after moving the rook
    public override IEnumerable<Move> GenerateMoves(State state) {
        foreach (Move generatedMove in base.GenerateMoves(state)) {
            State s = generatedMove.StateAfter;
            bool color = !s.WhiteIsActive;
            bool queenSide = color ? s.WhiteCastleQueenSide : s.BlackCastleQueenSide;
            bool kingSide = color ? s.WhiteCastleKingSide : s.BlackCastleKingSide;
            
            
            var rooksMask = color ? s.WhiteRooks : s.BlackRooks;
            if (queenSide) {
                s = RemoveCastles(rooksMask, s, color, false);
            }

            if (kingSide) {
                s = RemoveCastles(rooksMask, s, color, true);
            }

            yield return generatedMove with { StateAfter = s };
        }
    }

    private State RemoveCastles(Bitboard rooksMask, State stateBefore, bool color, bool kingQueen) {
        Bitboard castleMask = BitMask.Col[kingQueen ? 7 : 0] & BitMask.Row[color ? 0 : 7];
        if (!(rooksMask & castleMask).IsEmpty())
            return stateBefore;
        return (color, kingQueen) switch {
            (true, true) => stateBefore with { WhiteCastleKingSide = false },
            (false, true) => stateBefore with { BlackCastleKingSide = false },
            (true, false) => stateBefore with { WhiteCastleQueenSide = false },
            (false, false) => stateBefore with { BlackCastleQueenSide = false },

            // _ => throw new ArgumentOutOfRangeException()
        };

    }

    protected override Pieces WhitePiece => Pieces.WhiteRooks;
    protected override Pieces BlackPiece => Pieces.BlackRooks;
}