using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class EnginePlayer : IPlayer {
    private readonly MinimaxEvaluator _negamaxer = new();
    
    public SearchHandle GetBestMove(State state, Timers timers) {
        var cts = new CancellationTokenSource();

        var task = Task.Run(() =>
            _negamaxer.PrimitiveIterativeSearch(state, timers,cts.Token));

        return new SearchHandle(cts, task);
    }

}