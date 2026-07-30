using System.Diagnostics;
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;

namespace MacroBenchmarks;

class Program {
    static void Main(string[] args) {
        var state = State.Initial;

        var minimaxer = new MinimaxEvaluator();
        var cts = new CancellationTokenSource();
        var timers = Timers.Default;
        
        var sw = Stopwatch.StartNew();
        var result = minimaxer.PrimitiveIterativeSearch(state, timers,  cts.Token);
        var time = sw.Elapsed;
        Console.WriteLine($"Took {time} time.");
        Console.WriteLine(result);
    }
}
