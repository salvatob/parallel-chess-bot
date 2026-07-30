using BenchmarkDotNet.Attributes;
using ChessBotCore;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Performance;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerftBenchmarks {
    private State _state = null!;

    [ParamsSource(nameof(PositionCases))]
    public string Position { get; set; } = BenchmarkPositions.InitialPosition;

    [Params(1, 2, 3)] 
    public int Depth { get; set; }

    public static IEnumerable<string> PositionCases => BenchmarkPositions.PositionCases;

    [GlobalSetup]
    public void Setup() {
        _state = BenchmarkPositions.GetState(Position);
    }

    [Benchmark(Baseline = true)]
    public long SingleThreaded() {
        return Performance.Perft(_state, Depth);
    }

    [Benchmark]
    public long MultiThreaded() {
        return Performance.ParallelPerft(_state, Depth);
    }
}
