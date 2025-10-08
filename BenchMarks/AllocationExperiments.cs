using BenchmarkDotNet.Attributes;
using ChessBotCore;

namespace Benchmarks;

[MemoryDiagnoser]
// [SimpleJob(iterationCount: 3, warmupCount: 1)]
[SimpleJob(iterationCount: 2, warmupCount: 0, launchCount: 1)]
public class AllocationExperiments {

  private const int _depth = 3;
  private readonly State _state = State.Initial;
  
  [Benchmark]
  public long Single_Threaded() {
    var chess = new DefaultChessWrapper();
    return chess.Perft(_state, _depth);
  }
  
  [Benchmark]
  public long Multi_Threaded() {
    var chess = new ParallelChessWrapper();
    return chess.Perft(_state, _depth);
  }
}