using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ChessBotCore;
using ChessBotCore.Search;

namespace Benchmarks;

public class SearchConfig : ManualConfig {
    public SearchConfig() {
        AddColumn(new SearchCustomMetrics());
    }
}

[Config(typeof(SearchConfig))]
public class Search {

    private ParallelMinimaxer minimaxer = new();
    private MinimaxEvaluator negamaxer;


    [Params(6)] 
    public int Depth;

    public IEnumerable<State> states => new List<State> { State.Initial };
    
    [ParamsSource(nameof(states))]
    public State state;
    
    
    // private var parameters = (State.Initial, depth);
    
    [Benchmark(Baseline = true)]
    public int SimpleMinimax() {
        return minimaxer.Minimax(state, Depth);
    }
    
    [Benchmark]
    public int SimpleNegamax() {
        return negamaxer.Negamax(state, Depth);
    }

    [Benchmark]
    public int ABNegamax() {
        return negamaxer.ABNegamax(state, Depth, int.MinValue+2, int.MaxValue-2);
    }
    
    [Benchmark]
    public int SmartABNegamax() {
        return negamaxer.SmartABNegamax(state, Depth, int.MinValue+2, int.MaxValue-2);
    }
}