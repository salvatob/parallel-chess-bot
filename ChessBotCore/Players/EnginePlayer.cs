using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class EnginePlayer {
    private readonly MinimaxEvaluator _negamaxer = new();
    
    public SearchHandle StartSearch(State state, Timers timers) {
        var cts = new CancellationTokenSource(2000);

        var task = Task.Run(() =>
            _negamaxer.PrimitiveIterativeSearch(state, timers,cts.Token));

        return new SearchHandle(cts, task);
    }
    
}