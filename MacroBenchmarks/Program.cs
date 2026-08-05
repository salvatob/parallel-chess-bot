using System.Diagnostics;
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;

namespace MacroBenchmarks;

class MacroBenchmarks {
    static void Main(string[] args) {
        var state = State.Initial;

        var minimaxer = new MinimaxEvaluator();
        var cts = new CancellationTokenSource();
        var timers = Timers.Default;
        // var timers = new Timers() {
        //     BaseBlackTime =  TimeSpan.FromSeconds(2),
        //     BaseWhiteTime = TimeSpan.FromSeconds(2),
        //     Increment = TimeSpan.FromSeconds(1)
        // };
        
        var sw = Stopwatch.StartNew();
        var result = minimaxer.PrimitiveIterativeSearch(state, timers,  cts.Token);
        var time = sw.Elapsed;
        Console.WriteLine($"Took {time} time.");
        Console.WriteLine(result);
    }

    static void CompareItearativeAndABMinimax() {
        var depth = 7;
        
        var state = State.Initial;
        var minimaxer = new MinimaxEvaluator();
        
        var sw = new Stopwatch();
        
        sw.Start();
        var minimaxed = minimaxer.ChooseBestMove(state, depth);
        var mmTime = sw.Elapsed;
        Console.WriteLine($"Took {mmTime} time.");
        sw.Restart();
        
        // var itarated = minimaxer.PrimitiveIterativeSearch()
        
    }
}
