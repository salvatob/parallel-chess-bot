using BenchmarkDotNet.Attributes;
using ChessBotCore;
using ChessBotCore.ChessWrappers;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerftBenchmarks {
  private State _state;
  private DefaultChessWrapper _defaultChess = null!;
  private ParallelChessWrapper _parallelChess = null!;

  [ParamsSource(nameof(PositionCases))]
  public string Position { get; set; } = BenchmarkPositions.InitialPosition;

  [Params(1, 2, 3)]
  public int Depth { get; set; }

  public static IEnumerable<string> PositionCases => BenchmarkPositions.PositionCases;

  [GlobalSetup]
  public void Setup() {
    _state = BenchmarkPositions.GetState(Position);
    _defaultChess = new DefaultChessWrapper();
    _parallelChess = new ParallelChessWrapper();
  }

  [Benchmark(Baseline = true)]
  public long SingleThreaded() {
    return _defaultChess.Perft(_state, Depth);
  }

  [Benchmark]
  public long MultiThreaded() {
    return _parallelChess.Perft(_state, Depth);
  }
}
