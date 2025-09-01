namespace ChessBotCore;

public interface IChessWrapper {
    public long Perft(State state, int depth);

    public long EvalPerft(State state, int depth);
    public Task<Move> GetBestMove(State state, int timeMs);
}