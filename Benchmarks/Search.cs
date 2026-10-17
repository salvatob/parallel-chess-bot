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
    private readonly ParallelMinimaxer minimaxer = new();
    private MinimaxEvaluator.SearchContext _context;


    [Params(6)] public int Depth;

    private MinimaxEvaluator negamaxer;

    [ParamsSource(nameof(states))]
    public State state;

    public IEnumerable<State> states => new List<State> { State.Initial };

    // private var parameters = (State.Initial, depth);

    [Benchmark(Baseline = true)]
    public SearchScore SimpleMinimax() {
        return minimaxer.Minimax(state, Depth);
    }

    [Benchmark]
    public SearchScore SimpleNegamax() {
        return negamaxer.Negamax(state, Depth);
    }
    //
    // [Benchmark]
    // public SearchScore ABNegamax() {
    //     return negamaxer.ABNegamax(state, Depth, int.MinValue + 1, int.MaxValue);
    // }

    // [Benchmark]
    // ReSharper disable once InconsistentNaming
    // public SearchScore SmartABNegamax() {
    //     return negamaxer.SmartABNegamax(state, Depth, _context, int.MinValue + 1, int.MaxValue);
    // }
}
