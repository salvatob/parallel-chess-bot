using BenchmarkDotNet.Attributes;
using ChessBotCore;
using ChessBotCore.MoveGenerators;

namespace Benchmarks;

[MemoryDiagnoser]
public class AllocationExperiments {
    private State _state;
    private readonly IMoveGenerator _generator = new GeneratorWrapper();

    [ParamsSource(nameof(PositionCases))]
    public string Position { get; set; } = BenchmarkPositions.InitialPosition;

    public static IEnumerable<string> PositionCases => BenchmarkPositions.PositionCases;

    [GlobalSetup]
    public void Setup() {
        _state = BenchmarkPositions.GetState(Position);
    }

    [Benchmark]
    public List<Move> LegalMovesToList() {
        return _generator.GenerateMoves(_state).GetLegalMoves();

    }

    [Benchmark(Baseline = true)]
    public List<Move> PseudoLegalMovesToList() {
        return _generator.GenerateMoves(_state).GetAllMoves();
        
    }
}
