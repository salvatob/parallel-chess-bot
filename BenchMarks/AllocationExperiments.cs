using BenchmarkDotNet.Attributes;
using ChessBotCore;

namespace Benchmarks;

[MemoryDiagnoser]
// [SimpleJob(iterationCount: 3, warmupCount: 1)]
[SimpleJob(iterationCount: 5, warmupCount: 0, launchCount: 1)]
public class AllocationExperiments {

  private const int _depth = 3;
  private readonly State _state = State.Initial;
  private readonly GeneratorWrapper _gen = GeneratorWrapper.Default;
  
  
  // [Benchmark]
  public long Single_Threaded() {
    var chess = new DefaultChessWrapper();
    return chess.Perft(_state, _depth);
  }
  
  // [Benchmark]
  public long Multi_Threaded() {
    var chess = new ParallelChessWrapper();
    return chess.Perft(_state, _depth);
  }

  [Benchmark]
  public List<Move> NormalGeneration() {
    return _gen.GetLegalMoves(_state).ToList();
  }
  
  // [Benchmark]
  // public List<Move> FastGeneration() {
  //   return _gen.GetLegalMovesFast(_state).ToList();
  // }
}