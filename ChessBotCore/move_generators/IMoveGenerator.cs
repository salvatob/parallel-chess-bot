namespace ChessBotCore;

public interface IMoveGenerator {
    public static abstract List<Move> GenerateMoves(State state);

    public static List<Bitboard> SplitIntoMoves(Bitboard pieces, Bitboard piecesAfter) {
        return [];
    }
}