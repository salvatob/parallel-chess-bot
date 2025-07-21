namespace ChessBotCore;

public interface IMoveGenerator {
    public static abstract List<State> GenerateMoves(State state);

    public static List<Bitboard> SplitIntoMoves(Bitboard pieces, Bitboard piecesAfter) {
        return [];
    }
}