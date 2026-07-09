using BenchmarkDotNet.Attributes;
using ChessBotCore;

namespace Benchmarks;

public class Search{

    private ParallelMinimaxer minimaxer = new ParallelMinimaxer();
    private MinimaxEvaluator negamaxer = new MinimaxEvaluator();


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