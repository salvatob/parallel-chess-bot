using ChessBotCore.Search;

namespace ChessBotCore.ChessWrappers;

public interface IChessWrapper {
    public long Perft(State state, int depth);

    public long EvalPerft(State state, int depth);
    public Task<SearchResults> GetBestMove(State state, int timeMs);
}