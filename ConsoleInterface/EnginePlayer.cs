using ChessBotCore;
using ChessBotCore.Search;

namespace ConsoleInterface;


public interface IPlayer {
    public SearchHandle GetBestMove(State state, Timers timers);
}


public class EnginePlayer {
    MinimaxEvaluator _negamaxer = new MinimaxEvaluator(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(2));
    
    
    public SearchHandle StartSearch(State state, TimeSpan time) {
        var cts = new CancellationTokenSource(time/2);

        var task = Task.Run(() =>
            _negamaxer.PrimitiveIterativeSearch(state, cts.Token, time));

        return new SearchHandle(cts, task);
    }
}