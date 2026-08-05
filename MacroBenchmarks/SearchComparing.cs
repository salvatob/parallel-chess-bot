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
        var depth = 9;
        
        var state = State.Initial;
        var minimaxer = new MinimaxEvaluator();
        
        var sw = new Stopwatch();
        
        sw.Start();
        var minimaxed = minimaxer.ChooseBestMove(state, depth);
        var mmTime = sw.Elapsed;
        
        Console.WriteLine($"Minimax: Took {mmTime} time.");
        Console.WriteLine(minimaxed);
        sw.Reset();
        
        var iteratorTimeManager = new MockTimeManager(mmTime);
        var iterator = new MinimaxEvaluator(iteratorTimeManager);
        var uselessTimers = new Timers();
        using var cts = new CancellationTokenSource();
        
        sw.Start();
        var iterated = iterator.PrimitiveIterativeSearch(state, uselessTimers, cts.Token );
        var itTime =  sw.Elapsed;
        Console.WriteLine($"Iterated: Took {itTime} time.");
        Console.WriteLine(iterated);
        
    }
}