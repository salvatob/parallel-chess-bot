using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class EnginePlayer : IPlayer {
    private readonly MinimaxEvaluator _negamaxer = new();
    
    public SearchHandle ChooseMoveAsync(State state, Timers timers) {
        var cts = new CancellationTokenSource();

        var task = Task.Run(() =>
            _negamaxer.PrimitiveIterativeSearch(state, timers,cts.Token));

        return new SearchHandle(cts, task);
    }

    public Task OnGameStartAsync(bool yourColor, State state) {
        return Task.CompletedTask;
    }
    public Task OnGameGameEndAsync(bool yourColor, GameResult result) {
        return Task.CompletedTask;
    }
    
    public void Dispose() {
        // its totally okay that its empty for now
        // in future, this could save the results into some kind of log file so I can inspect the games.
    }
}