namespace ChessBotCore.Game;

/// <summary>
/// Allows comparing two <see cref="State"/>s for equality relevant to the 3-fold repetition rule,
/// meaning states are considered equal, if they have all pieces on the same squares, and they have the same possible moves.
/// The equality DOES NOT take into account move clocks.
/// </summary>
public class ThreeFoldRepetitionStateComparer : IEqualityComparer<State> {
    public bool Equals(State? x, State? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return GetStatePieceInfo(x).Equals(GetStatePieceInfo(y));
    }
    public int GetHashCode(State s) {
        return GetStatePieceInfo(s).GetHashCode();
    }

    private static object GetStatePieceInfo(State s) {
        return (
            s.WhitePawns, s.WhiteRooks, s.WhiteKnights, s.WhiteBishops, s.WhiteQueens, s.WhiteKing,
            s.BlackPawns, s.BlackRooks, s.BlackKnights, s.BlackBishops, s.BlackQueens, s.BlackKing,
            s.WhiteCastleKingSide, s.WhiteCastleQueenSide,
            s.BlackCastleKingSide, s.BlackCastleQueenSide,
            s.WhiteIsActive,
            s.EnPassant
        );
    }
}
