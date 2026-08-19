using System.Diagnostics;
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;

namespace MacroBenchmarks;

class MockTimeManager : ITimeManager {
    private TimeSpan _recommendedTime;
    
    public MockTimeManager(TimeSpan recommendedTime) => _recommendedTime = recommendedTime;
    
    public TimeSpan CalculateAllowedTime(Timers timers, bool isWhite) {
        return _recommendedTime;
    }
}

public class SearchComparing {
    
    public static void CompareItearativeAndABMinimax() {
        var depth = 10;
        
        var state1 = State.Initial;
        var state2 = state1.Clone();
        var minimaxer = new MinimaxEvaluator();
        
        var sw = new Stopwatch();
        
        sw.Start();
        var minimaxed = minimaxer.ChooseBestMove(state1, depth);
        var mmTime = sw.Elapsed;
        
        Console.WriteLine($"Minimax: Took {mmTime} time.");
        Console.WriteLine(minimaxed);
        sw.Reset();
        
        
        var iteratorTimeManager = new MockTimeManager(mmTime);
        var iterator = new MinimaxEvaluator(iteratorTimeManager);
        var uselessTimers = new Timers();
        using var cts = new CancellationTokenSource();
        
        sw.Start();
        var iterated = iterator.PrimitiveIterativeSearch(state2, uselessTimers, cts.Token );
        var itTime =  sw.Elapsed;
        Console.WriteLine($"Iterated: Took {itTime} time.");
        Console.WriteLine(iterated);
        
    }
}